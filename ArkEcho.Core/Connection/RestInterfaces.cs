using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public interface IRest
    {
        int Timeout { get; set; }

        Task<bool> CheckConnection();
    }

    public interface IRestLogging : IRest
    {
        Task<bool> PostLogging(LogMessage logMessage);
    }

    public interface IRestUser : IRest
    {
        Task<User> AuthenticateUserForLogin(User userToAuthenticate);

        Task<User> CheckUserToken(Guid guid);
    }

    public interface IRestMusic : IRest
    {
        Task<string> GetMusicLibrary();

        Task<string> GetAlbumCover(Guid guid);
    }

    public interface IRestFiles : IRest
    {
        Task<MemoryStream> GetFile(TransferFileBase tfb);
    }
}