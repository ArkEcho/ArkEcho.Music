using ArkEcho.Core;
using ArkEcho.Server.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public sealed class Server : IDisposable
    {
        private const string serverConfigFileName = "ServerConfig.json";

        private MusicLibraryManager libraryManager = null;

        private ManualResetEventSlim libraryWorkerEvent = new ManualResetEventSlim(false);
        private Logger logger = null;

        private IDatabaseAccess dbAccess = null;

        private List<User> loggedInUsers = new List<User>();
        private List<TokenInstance> apiTokens = new List<TokenInstance>(); // TODO: Timeout

        private ServerConfig serverConfig = null;

        public FileLoggingWorker LoggingWorker { get; private set; } = null;

        public AppEnvironment Environment { get; private set; } = null;
        public Server()
        {
            dbAccess = new SqliteDatabaseAccess();
        }

        public async Task<bool> Init()
        {
            Environment = new AppEnvironment(Resources.ARKECHOSERVER, Debugger.IsAttached, Resources.Platform.Server, true);
            Console.WriteLine($"Running in {(Environment.Development ? "Development" : "Production")}, Address={Environment.ServerAddress}");

            serverConfig = new ServerConfig(serverConfigFileName);
            if (!serverConfig.LoadFromFile(AppContext.BaseDirectory, true).Result)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping Server");
                return false;
            }

            try
            {
                await dbAccess.ConnectToDatabase(serverConfig.DatabasePath.LocalPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // We have the config -> initialize logging
            LoggingWorker = new FileLoggingWorker(serverConfig.LoggingFolder.LocalPath, serverConfig.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new FileLogger(Environment, "Main", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.Server:");
            logger.LogStatic($"\r\n{serverConfig.SaveToJsonString().Result}");

            libraryManager = new MusicLibraryManager(new FileLogger(Environment, "MusicManager", LoggingWorker));
            libraryManager.Finished += LibraryManager_Finished;
            libraryManager.LoadUserLibraries(await dbAccess.GetUsersAsync());

            libraryWorkerEvent.Wait();

            //for (int i = 0; i < 5000000; i++)
            //    logger.LogStatic($"LOREM IPSUM BLA UND BLUB; DAT IST EIN TEXT!");

            return true;
        }

        private void LibraryManager_Finished(object sender, EventArgs e)
        {
            Console.WriteLine($"Music Libraries loaded");
            libraryWorkerEvent.Set();
        }

        public async Task<string> CmdListAllUsers()
        {
            var users = await dbAccess.GetUsersAsync();

            if (users.Count == 0)
                return "no users in database";

            string result = $"user count={users.Count}";
            foreach (var user in users)
            {
                result += $"\r\n{user.ID,-3} {user.UserName,-10} {(user.Settings.DarkMode ? "dark" : "light")} {user.MusicLibraryPath.AbsolutePath}";
                foreach (var item in user.Settings.MusicPathList)
                    result += $"\n\t{item.MachineName,-15} {item.Path.AbsolutePath}";
            }
            return result;
        }

        public async Task<string> CmdCreateUser(string userName, string password, string musiclibrarypath)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || !Path.Exists(musiclibrarypath))
                return "invalid input";

            User user = new User()
            {
                UserName = userName,
                Password = Encryption.EncryptSHA256(password),
                MusicLibraryPath = new Uri(musiclibrarypath)
            };

            string checkResult = await checkUser(user);
            if (!string.IsNullOrEmpty(checkResult))
                return checkResult;

            if (!await dbAccess.InsertUserAsync(user))
                return "error on creating new user!";

            return $"created user {user.UserName}";
        }

        private async Task<string> checkUser(User user)
        {
            List<User> users = await dbAccess.GetUsersAsync();
            if (users.Any(x => x.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) && x.ID != user.ID))
                return "username already in use";
            else if (user.MusicLibraryPath.AbsolutePath.Length > 255)
                return "musiclibrarypath is too long, max 255 char";
            else if (!Path.Exists(user.MusicLibraryPath.AbsolutePath))
                return "musiclibrarypath doesn't exist";
            else if (users.Any(x => x.MusicLibraryPath == user.MusicLibraryPath && x.ID != user.ID))
                return "musiclibrarypath already in use";

            return string.Empty;
        }

        public async Task<string> CmdUpdateUser(int id, string field, string newValue)
        {
            if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(newValue) || id <= 0)
                return "invalid input";

            User user = await dbAccess.GetUserAsync(id);
            if (user == null)
                return $"User ID {id} not found!";

            if (field.Equals("username", StringComparison.OrdinalIgnoreCase))
                user.UserName = newValue;
            else if (field.Equals("password", StringComparison.OrdinalIgnoreCase))
                user.Password = Encryption.EncryptSHA256(newValue);
            else if (field.Equals("musiclibrarypath", StringComparison.OrdinalIgnoreCase))
            {
                if (!Path.Exists(newValue))
                    return "invalid input for musiclibrarypath";
                user.MusicLibraryPath = new Uri(newValue);
            }

            string checkResult = await checkUser(user);
            if (!string.IsNullOrEmpty(checkResult))
                return checkResult;

            if (!await dbAccess.UpdateUserAsync(user))
                return "error updating user!";

            return "updated user";
        }

        public async Task<string> CmdDeleteUser(int userId)
        {
            if (userId <= 0)
                return "invalid id";

            if (!await dbAccess.DeleteUserAsync(userId))
                return "error on deleting user!";

            return $"deleted user id {userId}";
        }

        public async Task<User> AuthenticateUserForLoginAsync(string userName, string userPasswordEncrypted)
        {
            User user = await dbAccess.GetUserAsync(userName, userPasswordEncrypted);
            if (user == null)
                return null;

            user.SessionToken = Guid.NewGuid();
            loggedInUsers.Add(user);

            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                return false;

            return await dbAccess.UpdateUserAsync(user);
        }

        public User GetUserFromSessionToken(Guid sessionToken)
        {
            return loggedInUsers.Find(x => x.SessionToken.Equals(sessionToken));
        }

        public bool LogoutSession(Guid token)
        {
            return loggedInUsers.RemoveAll(x => x.SessionToken == token) > 0;
        }

        public bool CheckSession(Guid token)
        {
            return loggedInUsers.Any(x => x.SessionToken.Equals(token));
        }

        public bool CheckApiToken(Guid apiToken)
        {
            return apiTokens.Any(x => x.ApiToken.Equals(apiToken));
        }

        public Guid GetApiToken(Guid sessionToken)
        {
            User user = GetUserFromSessionToken(sessionToken);
            if (user == null)
                return Guid.Empty;

            TokenInstance apiToken = new TokenInstance(user.ID);
            apiTokens.Add(apiToken);
            return apiToken.ApiToken;
        }

        public MusicLibrary GetUserMusicLibrary(Guid apiToken)
        {
            TokenInstance token = apiTokens.Find(x => x.ApiToken == apiToken);
            if (token == null)
                return null;

            return libraryManager.GetMusicLibrary(token.UserID);
        }

        public bool UpdateMusicRating(Guid apiToken, Guid musicGuid, int rating)
        {
            MusicLibrary library = GetUserMusicLibrary(apiToken);
            if (library == null)
                return false;

            MusicFile musicFile = library.MusicFiles.Find(x => x.GUID == musicGuid);
            if (musicFile == null)
                return false;

            ShellFileAccess.SetRating(musicFile.FullPath, rating);
            return true;
        }

        public List<TransferFileBase> GetAllFiles(Guid apiToken)
        {
            MusicLibrary library = GetUserMusicLibrary(apiToken);
            if (library == null)
                return null;

            List<TransferFileBase> list = new();

            list.AddRange(library.MusicFiles);
            list.AddRange(library.Playlists);

            return list;
        }

        public MusicFile GetMusicFile(Guid apiToken, Guid guid)
        {
            MusicLibrary library = GetUserMusicLibrary(apiToken);
            return library != null ? library.MusicFiles.Find(x => x.GUID == guid) : null;
        }

        public string GetAlbumCover(Guid apiToken, Guid guid)
        {
            MusicLibrary library = GetUserMusicLibrary(apiToken);
            return library != null ? library.Album.Find(x => x.GUID == guid).Cover64 : null;
        }

        public string GetAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private bool disposed;
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbAccess?.DisconnectFromDatabase();

                    LoggingWorker?.Dispose();
                    LoggingWorker = null;

                    libraryManager?.Dispose();
                    libraryManager = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

    }
}
