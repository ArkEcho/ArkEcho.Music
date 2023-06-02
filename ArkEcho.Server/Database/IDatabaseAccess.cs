using ArkEcho.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArkEcho.Server.Database
{
    public interface IDatabaseAccess
    {
        Task<bool> ConnectToDatabase(string dbFilePath);

        void DisconnectFromDatabase();

        Task<List<User>> GetUsersAsync();

        Task<User> GetUserAsync(string username, string passwordEncrypted);

        Task<bool> UpdateUserAsync(User user);

        Task<bool> InsertUserAsync(User user);
    }
}
