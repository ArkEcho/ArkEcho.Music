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

        public static ArkEchoServer Instance { get; } = new ArkEchoServer();

        public ServerConfig ServerConfig { get; private set; } = null;

        private MusicLibrary library = null;

        private MusicWorker musicWorker = null;

        private List<User> users = new List<User>();


        private LoggingWorker lw = null;

        public IWebHost Host { get; set; }

        private ArkEchoServer()
        {
            library = new MusicLibrary();
            musicWorker = new MusicWorker();
        }

        public bool Init()
        {
            if (Initialized)
                return Initialized;

            Console.WriteLine("Initializing ArkEcho.Server");

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

            lw = new LoggingWorker(ServerConfig.LoggingFolder.LocalPath);
            lw.RunWorkerAsync();

            Console.WriteLine("Configuration for ArkEcho.Server:");
            Console.WriteLine(ServerConfig.SaveToJsonString().Result);

            musicWorker.RunWorkerCompleted += MusicWorker_RunWorkerCompleted;
            LoadMusicLibrary();

            Host = WebHost.CreateDefaultBuilder()
                            .UseUrls($"https://*:{ServerConfig.Port}")
                            .UseKestrel()
                            .UseStartup<Startup>()
                            .Build();

            Initialized = true;

            users.Add(new User() { UserName = "test", Password = Encryption.Encrypt("test") });

            ServerLogger logger = new ServerLogger("Main");
            ServerLogger logger2 = new ServerLogger("Rest");

            logger.LogStatic("Test1");
            logger2.LogError("Test2");
            logger.LogImportant("Test3");
            logger2.LogDebug("Test4");

            return Initialized;
        }

        public void AddLogMessage(LogMessage log)
        {
            lw.AddLogMessage(log);
        }

        public User CheckUserForLogin(User user)
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
            Console.WriteLine($"Worker Completed!");
            if (e.Result != null)
            {
                library = (MusicLibrary)e.Result;
                Console.WriteLine($"Found {library.MusicFiles.Count} Music Files");
            }
            else
            {
                Console.WriteLine("### Error loading Music Library, stopping!");
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

        public void Stop()
        {
            Host.StopAsync();
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
