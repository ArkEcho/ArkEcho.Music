using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public interface IRest
    {
        int Timeout { get; set; }

        // Control
        Task<bool> CheckConnection();

        // Logging
        Task<bool> PostLogging(LogMessage logMessage);

        // User
        Task<User> AuthenticateUserForLogin(User userToAuthenticate);

        Task<bool> LogoutUser(Guid guid);

        Task<User> CheckUserToken(Guid guid);

        Task<bool> UpdateUser(User userToUpdate);

        // Music
        Task<Guid> GetMusicLibraryGuid();

        Task<MusicLibrary> GetMusicLibrary();

        Task<string> GetAlbumCover(Guid guid);

        // File
        Task<MemoryStream> GetFile(TransferFileBase tfb);
    }
}