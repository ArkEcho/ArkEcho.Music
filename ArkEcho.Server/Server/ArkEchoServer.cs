using ArkEcho.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public sealed class ArkEchoServer : IDisposable
    {
        private const string serverConfigFileName = "ServerConfig.json";

        private IWebHost host = null;
        private MusicLibrary library = null;
        private MusicWorker musicWorker = null;
        private List<User> users = new List<User>();
        private Logger logger = null;

        /// <summary>
        /// SingleTon
        /// </summary>
        public static ArkEchoServer Instance { get; } = new ArkEchoServer();

        public ServerConfig ServerConfig { get; private set; } = null;

        public LoggingWorker LoggingWorker { get; private set; } = null;

        private ArkEchoServer()
        {
            library = new MusicLibrary();
        }

        public bool Init()
        {
            if (Initialized)
                return Initialized;

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // TODO: MusicFilePath in json with \\? C:\Users\steph\Music\ -> Exception

            ServerConfig = new ServerConfig(serverConfigFileName);
            if (!ServerConfig.LoadFromFile(executingLocation, true).Result)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping Server");
                return false;
            }
            else if (string.IsNullOrEmpty(ServerConfig.MusicFolder.LocalPath) || !Directory.Exists(ServerConfig.MusicFolder.LocalPath))
            {
                Console.WriteLine("### Music File Path not found! Enter Correct Path like: \"C:\\Users\\UserName\\Music\"");
                return false;
            }

            // We have the config -> initialize logging
            LoggingWorker = new ServerLoggingWorker(ServerConfig.LoggingFolder.LocalPath, (Logging.LogLevel)ServerConfig.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new Logger("Server", "Main", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.Server:");
            logger.LogStatic($"\r\n{ServerConfig.SaveToJsonString().Result}");

            musicWorker = new MusicWorker(LoggingWorker);
            musicWorker.RunWorkerCompleted += MusicWorker_RunWorkerCompleted;

            LoadMusicLibrary();

            host = WebHost.CreateDefaultBuilder()
                            .UseUrls($"https://*:{ServerConfig.Port}")
                            .UseKestrel()
                            .UseStartup<Startup>()
                            .Build();

            Initialized = true;

            users.Add(new User() { UserName = "test", Password = Encryption.Encrypt("test"), AccessToken = Guid.NewGuid() });

            //
            //for (int i = 0; i < 10000; i++)
            //    logger.LogStatic($"LOREM IPSUM BLA UND BLUB; DAT IST EIN TEXT!");

            return Initialized;
        }

        public User AuthenticateUserForLogin(User user)
        {
            return users.Find(x => x.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) && x.Password.Equals(Encryption.Encrypt(user.Password), StringComparison.OrdinalIgnoreCase));
        }

        public User CheckUserToken(Guid token)
        {
            return users.Find(x => x.AccessToken.Equals(token));
        }

        public void LoadMusicLibrary()
        {
            library = null;
            musicWorker.RunWorkerAsync(ServerConfig.MusicFolder.LocalPath);
        }

        private void MusicWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                library = (MusicLibrary)e.Result;
                logger.LogStatic($"Loaded {library.MusicFiles.Count} Music Files");
            }
            else
            {
                logger.LogError("### Error loading Music Library, stopping!");
                Stop();
            }
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

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
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
    }
}
