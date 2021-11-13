using ArkEcho.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ArkEcho.Server
{
    public sealed class ArkEchoServer : IDisposable
    {
        private const string serverConfigFileName = "Config.json";

        public static ArkEchoServer Instance { get; } = new ArkEchoServer();

        public ServerConfig Config { get; private set; } = null;

        private MusicLibrary library = null;

        private MusicWorker musicWorker = null;

        public List<User> Users = new List<User>();

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

            Config = new ServerConfig(serverConfigFileName);
            if (!Config.LoadFromFile(executingLocation, true))
            {
                Console.WriteLine("### No Config File found -> created new one, please configure. Stopping Server");
                return false;
            }
            else if (string.IsNullOrEmpty(Config.MusicFolder) || !Directory.Exists(Config.MusicFolder))
            {
                Console.WriteLine("### Music File Path not found! Enter Correct Path like: \"C:\\Users\\UserName\\Music\"");
                return false;
            }
            else
            {
                Console.WriteLine("Configuration for ArkEcho.Server:");
                Console.WriteLine(Config.SaveToJsonString());
            }

            musicWorker.RunWorkerCompleted += MusicWorker_RunWorkerCompleted;
            LoadMusicLibrary();

            // Start the WebHost, Server and Controllers
            // Set WebRoot for Static Files (css etc.), use xcopy for output
            Host = WebHost.CreateDefaultBuilder()
                            .UseUrls($"https://*:{Config.Port}")
                            .UseKestrel()
                            .UseStartup<Startup>()
                            //.UseWebRoot($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\wwwroot")
                            .Build();

            Initialized = true;

            return Initialized;
        }

        public void LoadMusicLibrary()
        {
            library = null;
            musicWorker.RunWorkerAsync(Config.MusicFolder);
        }

        private void MusicWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine($"Worker Completed!");
            if (e.Result != null)
                library = (MusicLibrary)e.Result;
            else
            {
                Console.WriteLine("### Error loading Music Library, stopping!");
                Stop();
            }
        }

        public string GetMusicLibraryString()
        {
            return library != null ? library.SaveToJsonString() : string.Empty;
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
