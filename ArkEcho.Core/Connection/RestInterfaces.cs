using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public interface IRest
    {
        int Timeout { get; set; }
        Guid ApiToken { get; set; }

        // Control
        Task<bool> CheckConnection();

        // Logging
        Task<bool> PostLogging(LogMessage logMessage);

        // User
        Task<User> AuthenticateUser(string userName, string userPasswordEnrypted);

        Task<User> GetUser(Guid sessionToken);

        Task<bool> LogoutSession(Guid sessionToken);

        Task<bool> CheckSession(Guid sessionToken);

        Task<bool> UpdateUser(User userToUpdate);

        Task<Guid> GetApiToken(Guid sessionToken);

        // Music
        Task<Guid> GetMusicLibraryGuid();

        Task<MusicLibrary> GetMusicLibrary();

        Task<string> GetAlbumCover(Guid guid);

        Task<bool> UpdateMusicRating(Guid guid, int rating);

        // File
        Task<MemoryStream> GetFile(TransferFileBase tfb);
    }
}