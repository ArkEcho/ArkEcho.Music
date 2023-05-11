using ArkEcho.Core;
using ArkEcho.Server.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public sealed class Server : IDisposable
    {
        private const string serverConfigFileName = "ServerConfig.json";

        private MusicLibrary library = null;
        private MusicLibraryWorker musicWorker = null;
        private Logger logger = null;

        private IDatabaseAccess dbAccess = null;
        private List<User> loggedInUsers = new List<User>();

        /// <summary>   
        /// SingleTon
        /// TODO: Remove Singleton
        /// </summary>
        public static Server Instance { get; } = new Server();

        public ServerConfig Config { get; private set; } = null;

        public FileLoggingWorker LoggingWorker { get; private set; } = null;

        public AppEnvironment Environment { get; private set; } = null;
        private Server()
        {
            library = new MusicLibrary();
            dbAccess = new SqliteDatabaseAccess();
        }

        public async Task<bool> Init()
        {
            if (Initialized)
                return Initialized;

            Environment = new AppEnvironment(Resources.ARKECHOSERVER, Debugger.IsAttached, Resources.Platform.Server, true);

            Config = new ServerConfig(serverConfigFileName);
            if (!Config.LoadFromFile(AppContext.BaseDirectory, true).Result)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping Server");
                return false;
            }
            else if (string.IsNullOrEmpty(Config.MusicFolder.LocalPath) || !Directory.Exists(Config.MusicFolder.LocalPath))
            {
                Console.WriteLine($"### Music File Path {Config.MusicFolder.LocalPath} not found! Enter Correct Path like: \"C:\\Users\\UserName\\Music\"");
                return false;
            }
            else if (Directory.GetFiles(Config.MusicFolder.LocalPath).Length == 0 && Directory.GetDirectories(Config.MusicFolder.LocalPath).Length == 0)
            {
                Console.WriteLine("### Given Music Directory is empty!");
                return false;
            }

            try
            {
                await dbAccess.ConnectToDatabase(Config.DatabasePath.LocalPath);

                var test = dbAccess.GetUsersAsync().Result;

                if (test.Count == 0)
                {
                    User user = new User()
                    {
                        UserName = "test",
                        Password = Encryption.EncryptSHA256("test")
                    };
                    user.Settings.MusicPathList.Add(new UserSettings.UserPath() { MachineName = System.Environment.MachineName, Path = new Uri(@"D:\_TEMP\Music") });
                    await dbAccess.InsertUserAsync(user);
                }

                test = await dbAccess.GetUsersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // We have the config -> initialize logging
            LoggingWorker = new FileLoggingWorker(Config.LoggingFolder.LocalPath, Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new FileLogger(Environment, "Main", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.Server:");
            logger.LogStatic($"\r\n{Config.SaveToJsonString().Result}");

            musicWorker = new MusicLibraryWorker(new FileLogger(Environment, "MusicWorker", LoggingWorker));
            musicWorker.RunWorkerCompleted += MusicLibraryWorker_RunWorkerCompleted;

            loadMusicLibrary();

            Initialized = true;

            //for (int i = 0; i < 5000000; i++)
            //    logger.LogStatic($"LOREM IPSUM BLA UND BLUB; DAT IST EIN TEXT!");

            return Initialized;
        }

        public async Task<User> AuthenticateUserForLoginAsync(User user)
        {
            List<User> users = await dbAccess.GetUsersAsync();
            User toLogin = users.Find(x => x.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) && x.Password.Equals(user.Password, StringComparison.OrdinalIgnoreCase));
            if (toLogin == null)
                return null;

            toLogin.AccessToken = Guid.NewGuid();
            loggedInUsers.Add(toLogin);

            return toLogin;
        }

        public void LogoutUser(Guid token)
        {
            loggedInUsers.RemoveAll(x => x.AccessToken == token);
        }

        public User CheckUserToken(Guid token)
        {
            return loggedInUsers.Find(x => x.AccessToken.Equals(token));
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                return false;

            return await dbAccess.UpdateUserAsync(user);
        }

        private void loadMusicLibrary()
        {
            library = null;
            musicWorker.RunWorkerAsync(Config.MusicFolder.LocalPath);
        }

        private void MusicLibraryWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                logger.LogError("### Error loading Music Library, stopping!");
                return;
            }

            library = (MusicLibrary)e.Result;

            Console.WriteLine($"#####################################################################");
            Console.WriteLine($"    Library loaded! {library.MusicFiles.Count} Music Files");
            Console.WriteLine($"#####################################################################");
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

        public bool Initialized { get; private set; } = false;

        public bool RestartRequested { get; private set; } = false;

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

        #region Dispose

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

        #endregion
    }
}
