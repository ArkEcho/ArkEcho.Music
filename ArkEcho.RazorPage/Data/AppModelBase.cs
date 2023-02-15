using ArkEcho.Core;
using ArkEcho.WebPage;
using System.Diagnostics;

namespace ArkEcho.RazorPage.Data
{
    public abstract class AppModelBase : IAppModel
    {
        public MusicLibrary Library { get; private set; }

        public abstract Player Player { get; }

        public abstract LibrarySync Sync { get; }

        public abstract string MusicFolder { get; }

        public AppEnvironment Environment { get; }
        public User AuthenticatedUser { get; private set; } = null;

        public IAppModel.Status AppStatus { get; private set; } = IAppModel.Status.Started;

        protected Rest rest = null;
        protected Logger logger = null;
        private Authentication authentication = null;

        public event Action StatusChanged;

        public AppModelBase(AppEnvironment environment, ILocalStorage localStorage)
        {
            Environment = environment;

            rest = new Rest($"https://192.168.178.20:5002", Environment.UserHttpClientHandler, false);

            logger = new RestLogger(Environment, "AppModel", rest);

            authentication = new Authentication(localStorage, rest);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            AuthenticatedUser = await authentication.GetAuthenticationState();
            return AuthenticatedUser != null;
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            AuthenticatedUser = await authentication.AuthenticateUserForLogin(username, Encryption.EncryptSHA256(password));
            return AuthenticatedUser != null;
        }

        public async Task LogoutUser()
        {
            if (Player.Playing)
                Player.Stop();

            SetStatus(IAppModel.Status.Started);

            await authentication.MarkUserAsLoggedOut(AuthenticatedUser.AccessToken);
            AuthenticatedUser = null;
        }

        protected void SetStatus(IAppModel.Status status)
        {
            AppStatus = status;
            StatusChanged?.Invoke();
        }

        public async Task<bool> InitializeOnLoad()
        {
            SetStatus(IAppModel.Status.Connecting);

            if (!await rest.CheckConnection())
            {
                SetStatus(IAppModel.Status.NotConnected);
                return false;
            }

            SetStatus(IAppModel.Status.Connected);

            logger.LogStatic($"Executing on {Environment.Platform}");

            if (!await initializePlayer())
                return false;

            return await LoadLibraryFromServer();
        }

        protected async Task<bool> LoadLibraryFromServer()
        {
            SetStatus(IAppModel.Status.LoadingLibrary);

            if (Library != null)
            {
                Guid serverLibraryGuid = await rest.GetMusicLibraryGuid();
                if (serverLibraryGuid == Library.GUID)
                    return true;
            }

            Stopwatch sw = Stopwatch.StartNew();

            Library = await rest.GetMusicLibrary();

            if (Library == null)
            {
                logger.LogError($"Error loading Library from Server");
                return false;
            }
            await Library.CreateAlbumFileMap();

            Console.WriteLine($"Loading Library {sw.ElapsedMilliseconds} ms");

            SetStatus(IAppModel.Status.LoadingAlbumCover);

            sw.Restart();

            foreach (Album album in Library.Album)
            {
                if (string.IsNullOrEmpty(album.Cover64))
                    album.Cover64 = await GetAlbumCover(album.GUID);
            }
            Console.WriteLine($"Cover {sw.ElapsedMilliseconds} ms");

            SetStatus(IAppModel.Status.Initialized);

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

        public async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return await rest.GetAlbumCover(albumGuid);
        }

        protected abstract Task<bool> initializePlayer();
        public abstract Task<bool> InitializeOnLogin();

        public abstract Task StartSynchronizeMusic();
        public abstract Task<bool> ChangeMusicFolder();
    }
}
