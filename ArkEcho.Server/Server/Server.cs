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

        private MusicLibrary library = null;
        private MusicLibraryWorker musicWorker = null;
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
            library = new MusicLibrary();
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
            else if (string.IsNullOrEmpty(serverConfig.MusicFolder.LocalPath) || !Directory.Exists(serverConfig.MusicFolder.LocalPath))
            {
                Console.WriteLine($"### Music File Path {serverConfig.MusicFolder.LocalPath} not found! Enter Correct Path like: \"C:\\Users\\UserName\\Music\"");
                return false;
            }
            else if (Directory.GetFiles(serverConfig.MusicFolder.LocalPath).Length == 0 && Directory.GetDirectories(serverConfig.MusicFolder.LocalPath).Length == 0)
            {
                Console.WriteLine("### Given Music Directory is empty!");
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

            musicWorker = new MusicLibraryWorker(new FileLogger(Environment, "MusicWorker", LoggingWorker));
            musicWorker.RunWorkerCompleted += MusicLibraryWorker_RunWorkerCompleted;

            library = null;
            musicWorker.RunWorkerAsync(serverConfig.MusicFolder.LocalPath);

            libraryWorkerEvent.Wait();

            //for (int i = 0; i < 5000000; i++)
            //    logger.LogStatic($"LOREM IPSUM BLA UND BLUB; DAT IST EIN TEXT!");

            return library != null;
        }

        public async Task<string> CmdListAllUsers()
        {
            var users = await dbAccess.GetUsersAsync();

            if (users.Count == 0)
                return "no users in database";

            string result = $"user count={users.Count}";
            foreach (var user in users)
            {
                result += $"\r\n{user.ID,-3} {user.UserName,-10} {(user.Settings.DarkMode ? "dark" : "light")}";
                foreach (var item in user.Settings.MusicPathList)
                    result += $"\n\t{item.MachineName,-15} {item.Path.AbsolutePath}";
            }
            return result;
        }

        public async Task<string> CmdCreateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return "empty username or password!";

            User user = new User()
            {
                UserName = userName,
                Password = Encryption.EncryptSHA256(password)
            };

            if (!await dbAccess.InsertUserAsync(user))
                return "error on creating new user!";

            return $"created user {userName}";
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
            if (!CheckSession(sessionToken))
                return Guid.Empty;

            TokenInstance apiToken = new TokenInstance();
            apiTokens.Add(apiToken);
            return apiToken.ApiToken;
        }

        public bool UpdateMusicRating(Guid musicGuid, int rating)
        {
            if (library == null)
                return false;

            MusicFile musicFile = library.MusicFiles.Find(x => x.GUID == musicGuid);
            if (musicFile == null)
                return false;

            ShellFileAccess.SetRating(musicFile.FullPath, rating);
            return true;
        }

        private void MusicLibraryWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
                logger.LogError("Error loading Music Library");
            else
            {

                library = (MusicLibrary)e.Result;
                Console.WriteLine($"Library loaded {library.MusicFiles.Count} Music Files");
                logger.LogStatic($"Library loaded, {library.MusicFiles.Count} Music Files");
            }
            libraryWorkerEvent.Set();
        }

        public List<TransferFileBase> GetAllFiles()
        {
            if (library == null)
                return null;

            List<TransferFileBase> list = new();

            list.AddRange(library.MusicFiles);
            list.AddRange(library.Playlists);

            return list;
        }

        public MusicLibrary GetMusicLibrary()
        {
            return library;
        }

        public MusicFile GetMusicFile(Guid guid)
        {
            return library != null ? library.MusicFiles.Find(x => x.GUID == guid) : null;
        }

        public string GetAlbumCover(Guid guid)
        {
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

                    musicWorker?.Dispose();
                    musicWorker = null;
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
