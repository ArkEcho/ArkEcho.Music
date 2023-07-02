
using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Server
{
    public class MusicLibraryManager : IDisposable
    {
        private List<MusicLibraryWorker> workerPool = new List<MusicLibraryWorker>();
        private Logger logger;

        private const int MAXWORKER = 4;

        private List<LibraryData> libraryData = new List<LibraryData>();
        private bool disposedValue;

        public event EventHandler Finished;

        public MusicLibraryManager(Logger logger)
        {
            this.logger = logger;

            for (int i = 0; i < Math.Min(MAXWORKER, Environment.ProcessorCount / 2); i++)
            {
                var worker = new MusicLibraryWorker((i + 1) * 100, logger);
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                workerPool.Add(worker);
            }
        }

        public MusicLibrary GetMusicLibrary(int userID)
        {
            return libraryData.Find(x => x.UserID == userID)?.Library;
        }

        public void LoadUserLibraries(List<User> userList)
        {
            lock (libraryData)
            {
                foreach (User user in userList)
                {
                    libraryData.Add(new LibraryData()
                    {
                        UserID = user.ID,
                        Path = user.MusicLibraryPath.AbsolutePath
                    });
                }
            }

            if (libraryData.Count == 0)
            {
                logger.LogStatic($"No libraries to load");
                Finished?.Invoke(this, null);
                return;
            }

            startWorker();
        }

        private void startWorker()
        {
            lock (libraryData)
            {
                LibraryData data = libraryData.First(x => !x.InProgress && x.Library == null);
                if (data == null)
                    return;

                MusicLibraryWorker worker = workerPool.First(x => !x.IsBusy);
                if (worker == null)
                    return;

                logger.LogDebug($"Starting Worker ID={worker.ID} on Path={data.Path}");
                data.InProgress = true;
                worker.RunWorkerAsync(data);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lock (libraryData)
            {
                if (libraryData.Any(x => !x.InProgress && x.Library == null))
                    startWorker();
                else
                {
                    logger.LogDebug($"All workers finished and libraries loaded");
                    Finished?.Invoke(this, null);
                }
            }
        }

        public class LibraryData
        {
            public int UserID { get; set; } = 0;
            public MusicLibrary Library { get; set; } = null;
            public string Path { get; set; }
            public bool InProgress { get; set; } = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    workerPool.ForEach(x => x.Dispose());
                    workerPool.Clear();
                    workerPool = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
