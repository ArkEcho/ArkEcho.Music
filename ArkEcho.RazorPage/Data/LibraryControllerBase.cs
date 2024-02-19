using ArkEcho.Core;
using System.Diagnostics;

namespace ArkEcho.RazorPage.Data
{
    public abstract class LibraryControllerBase
    {
        public MusicLibrary Library { get; private set; }

        protected Rest rest = null;
        protected Logger logger;

        public event Action LibraryLoaded;

        public LibraryControllerBase(Logger logger, Rest rest)
        {
            this.rest = rest;
            this.logger = logger;
        }

        public async virtual Task<bool> LoadLibraryFromServer()
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

            if (Library.MusicFiles.Count <= 0)
            {
                logger.LogStatic($"Error initializing Library/Music Count is Zero!");
                return false;
            }

            logger.LogStatic($"Library initialized, {Library.MusicFiles.Count}");
            LibraryLoaded?.Invoke();
            return true;
        }

        public async Task<bool> UpdateMusicRating(Guid musicFileGuid, int rating)
        {
            return await rest.UpdateMusicRating(musicFileGuid, rating);
        }

        public virtual async void Dispose()
        {
            Library = null;
        }
    }
}
