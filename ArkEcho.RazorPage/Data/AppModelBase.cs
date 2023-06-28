using ArkEcho.Core;
using ArkEcho.WebPage;
using System.Diagnostics;

namespace ArkEcho.RazorPage.Data
{
    public abstract class AppModelBase : IAppModel
    {
        public MusicLibrary Library { get; private set; }

        public abstract Player Player { get; protected set; }

        public abstract LibrarySync Sync { get; }

        public abstract string MusicFolder { get; }

        public AppEnvironment Environment { get; }
        public User AuthenticatedUser { get; private set; } = null;

        public IAppModel.Status AppStatus { get; private set; } = IAppModel.Status.Started;

        protected Rest rest = null;
        protected Logger logger = null;
        protected SnackbarDialogService snackbarDialogService = null;
        private Authentication authentication = null;

        public event Action StatusChanged;

        public AppModelBase(AppEnvironment environment, ILocalStorage localStorage)
        {
            Environment = environment;

            rest = new Rest(environment.ServerAddress, Environment.UserHttpClientHandler);

            logger = new RestLogger(Environment, "AppModel", rest);

            authentication = new Authentication(localStorage, rest);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            return await authentication.GetAuthenticationState();
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            if (await IsUserAuthenticated())
                return true;

            AuthenticatedUser = await authentication.AuthenticateUserForLogin(username, password);
            return AuthenticatedUser != null;
        }

        public virtual async Task LogoutUser()
        {
            if (Player.Playing)
                Player.Stop();
            Player.Reset();

            SetStatus(IAppModel.Status.Started);

            await authentication.LogoutUser();

            AuthenticatedUser = null;
        }

        protected void SetStatus(IAppModel.Status status)
        {
            AppStatus = status;
            StatusChanged?.Invoke();
        }

        public async Task<bool> InitializeOnLoad()
        {
            if (!await rest.CheckConnection())
            {
                SetStatus(IAppModel.Status.NotConnected);
                return false;
            }

            SetStatus(IAppModel.Status.Connected);
            return true;
        }

        protected async Task<bool> LoadLibraryFromServer()
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
            if (!await initializePlayer())
                return false;

            SetStatus(IAppModel.Status.LoadingLibrary);

            if (!await LoadLibraryFromServer())
                return false;

            SetStatus(IAppModel.Status.Authorized);
            return true;
        }

        protected abstract Task<bool> initializePlayer();

        public abstract Task StartSynchronizeMusic();
        public abstract Task<bool> ChangeMusicFolder();

        public void SetSnackbarDialogService(SnackbarDialogService snackbarDialogService) => this.snackbarDialogService = snackbarDialogService;
    }
}
