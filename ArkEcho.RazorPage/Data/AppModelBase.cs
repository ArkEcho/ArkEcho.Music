using ArkEcho.Core;
using System.Diagnostics;

namespace ArkEcho.RazorPage.Data
{
    public abstract class AppModelBase
    {
        public enum Status
        {
            Started = 0,

            NotConnected = 10,
            Connected = 20,

            LoadingLibrary = 20,

            Authorized = 50,
        }

        public MusicLibrary Library { get; private set; }

        public Status AppStatus { get; private set; } = Status.Started;

        protected Rest rest = null;
        protected Logger logger;

        private Authentication authentication = null;

        public event Action StatusChanged;

        public AppModelBase(Logger logger, Rest rest)
        {
            this.rest = rest;
            this.logger = logger;
        }

        public virtual async Task LogoutUser()
        {
            SetStatus(Status.Started);
        }

        protected void SetStatus(Status status)
        {
            AppStatus = status;
            StatusChanged?.Invoke();
        }

        public async Task<bool> CheckConnection()
        {
            if (!await rest.CheckConnection())
            {
                SetStatus(Status.NotConnected);
                return false;
            }

            SetStatus(Status.Connected);
            return true;
        }

        public async Task<bool> LoadLibraryFromServer()
        {
            if (Library != null)
            {
                Guid serverLibraryGuid = await rest.GetMusicLibraryGuid();
                if (serverLibraryGuid == Library.GUID)
                {
                    logger.LogDebug($"Library already loaded and synced with Server");
                    return true;
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            Library = await rest.GetMusicLibrary();

            if (Library == null)
            {
                logger.LogError($"Error loading Library from Server");
                return false;
            }
            await Library.CreateAlbumFileMap();

            sw.Restart();

            foreach (Album album in Library.Album)
            {
                if (string.IsNullOrEmpty(album.Cover64))
                    album.Cover64 = await rest.GetAlbumCover(album.GUID);
            }

            if (Library.MusicFiles.Count > 0)
            {
                logger.LogStatic($"Library initialized, {Library.MusicFiles.Count}");
                return true;
            }
            else
            {
                logger.LogStatic($"Error initializing Library/Music Count is Zero!");
                return false;
            }
        }

        public async Task<bool> UpdateMusicRating(Guid musicFileGuid, int rating)
        {
            return await rest.UpdateMusicRating(musicFileGuid, rating);
        }

        public virtual async Task<bool> InitializeOnLogin()
        {
            SetStatus(Status.LoadingLibrary);

            if (!await LoadLibraryFromServer())
                return false;

            SetStatus(Status.Authorized);
            return true;
        }
    }
}
