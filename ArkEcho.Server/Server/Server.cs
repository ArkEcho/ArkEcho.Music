using ArkEcho.Core;
using ArkEcho.Server.Database;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    // TODO: Eigene Playlists erstellen speichern verwalten
    // TODO: Playlists ändern und über Rest übertragen
    public sealed class Server : IDisposable
    {
        private const string serverConfigFileName = "ServerConfig.json";

        private IWebHost host = null;
        private MusicLibrary library = null;
        private MusicLibraryWorker musicWorker = null;
        private Logger logger = null;

        private IDatabaseAccess dbAccess = null;
        private List<User> loggedInUsers = new List<User>();

        /// <summary>   
        /// SingleTon
        /// </summary>
        public static Server Instance { get; } = new Server();

        public ServerConfig Config { get; private set; } = null;

        public FileLoggingWorker LoggingWorker { get; private set; } = null;

        private Server()
        {
            library = new MusicLibrary();
            dbAccess = new SqliteDatabaseAccess();
        }

        public async Task<bool> Init()
        {
            if (Initialized)
                return Initialized;

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Config = new ServerConfig(serverConfigFileName);
            if (!Config.LoadFromFile(executingLocation, true).Result)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping Server");
                return false;
            }
            else if (string.IsNullOrEmpty(Config.MusicFolder.LocalPath) || !Directory.Exists(Config.MusicFolder.LocalPath))
            {
                Console.WriteLine("### Music File Path not found! Enter Correct Path like: \"C:\\Users\\UserName\\Music\"");
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
                    await dbAccess.InsertUserAsync(new User() { UserName = "test", Password = Encryption.EncryptSHA256("test") });

                //test[0].UserName = "BLUB";
                //bool up = await dbAccess.UpdateUserAsync(test[0]);
                //test = await dbAccess.GetUsersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // We have the config -> initialize logging
            LoggingWorker = new FileLoggingWorker(Config.LoggingFolder.LocalPath, Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new Logger(Resources.ARKECHOSERVER, "Main", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.Server:");
            logger.LogStatic($"\r\n{Config.SaveToJsonString().Result}");

            musicWorker = new MusicLibraryWorker(LoggingWorker);
            musicWorker.RunWorkerCompleted += MusicLibraryWorker_RunWorkerCompleted;

            loadMusicLibrary();

            host = WebHost.CreateDefaultBuilder()
                            .UseUrls($"https://*:{Config.Port}")
                            .UseKestrel()
                            .UseStartup<Startup>()
                            .Build();

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

            user.AccessToken = Guid.NewGuid();
            loggedInUsers.Add(user);

            return user;
        }

        public User CheckUserToken(Guid token)
        {
            return loggedInUsers.Find(x => x.AccessToken.Equals(token));
        }

        private void loadMusicLibrary()
        {
            library = null;
            musicWorker.RunWorkerAsync(Config.MusicFolder.LocalPath);
        }

        private void MusicLibraryWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
                library = (MusicLibrary)e.Result;
            else
            {
                logger.LogError("### Error loading Music Library, stopping!");
                Stop();
            }
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

        public async Task<string> GetMusicLibraryString()
        {
            return library != null ? await library.SaveToJsonString() : string.Empty;
        }

        public MusicFile GetMusicFile(Guid guid)
        {
            return library != null ? library.MusicFiles.Find(x => x.GUID == guid) : null;
        }

        public string GetAlbumCover(Guid guid)
        {
            return library != null ? library.Album.Find(x => x.GUID == guid).Cover64 : null;
        }

        public void Start()
        {
            host.Run();
        }

        public void Stop()
        {
            host.StopAsync();
        }

        public void Restart()
        {
            RestartRequested = true;
            Stop();
        }

        public bool Initialized { get; private set; } = false;

        public bool RestartRequested { get; private set; } = false;

        #region Dispose

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbAccess?.DisconnectFromDatabase();

                    musicWorker?.Dispose();
                    musicWorker = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
