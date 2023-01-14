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

        Task<bool> UpdateUserAsync(User user);

        Task<bool> InsertUserAsync(User user);
    }
}
