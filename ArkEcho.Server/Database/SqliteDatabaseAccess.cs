using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server.Database
{
    public class SqliteDatabaseAccess : IDatabaseAccess
    {
        private SQLiteConnection connection = null;

        public async Task<bool> ConnectToDatabase(string dbFilePath)
        {
            if (!dbFilePath.EndsWith(".sqlite"))
                throw new Exception($"Invalid Filename, must end with '.sqlite'");

            connection = new SQLiteConnection($"DataSource={dbFilePath};Version=3;");

            bool dbExists = File.Exists(dbFilePath);
            if (!dbExists)
                SQLiteConnection.CreateFile(dbFilePath);

            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                DisconnectFromDatabase();
                return false;
            }

            if (!dbExists)
                await createDB(dbFilePath);

            return true;
        }

        private async Task createDB(string dbFilePath)
        {
            if (connection == null)
                throw new Exception($"Not Connected to Database!");

            string sql = $"create table {User.UserTableName} (id integer primary key autoincrement, username varchar(30), password varchar(30))";

            SQLiteCommand command = new SQLiteCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        public void DisconnectFromDatabase()
        {
            connection?.Close();
            connection?.Dispose();
            connection = null;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            if (connection == null)
                throw new Exception($"Not Connected to Database!");

            string sql = $"select * from {User.UserTableName}";

            using SQLiteCommand command = new SQLiteCommand(sql, connection);
            using DbDataReader reader = await command.ExecuteReaderAsync();

            Dictionary<string, int> keyValues = getFieldValueMap<User.UserTable>(reader);

            List<User> users = new List<User>();
            while (await reader.ReadAsync())
            {
                User user = new();
                user.ID = reader.GetInt32(keyValues[User.UserTable.ID.ToString()]);
                user.UserName = reader.GetString(keyValues[User.UserTable.USERNAME.ToString()]);
                user.Password = reader.GetString(keyValues[User.UserTable.PASSWORD.ToString()]);
                users.Add(user);
            }
            return users;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (connection == null)
                throw new Exception($"Not Connected to Database!");

            string sql = $"update {User.UserTableName} set username = '{user.UserName}', password = '{user.Password}' where id = {user.ID}";

            using SQLiteCommand command = new SQLiteCommand(sql, connection);
            return await command.ExecuteNonQueryAsync() != 1;
        }

        public async Task<bool> InsertUserAsync(User user)
        {
            if (connection == null)
                throw new Exception($"Not Connected to Database!");

            string sql = $"insert into {User.UserTableName} (username, password) values ('{user.UserName}', '{user.Password}')";

            using SQLiteCommand command = new SQLiteCommand(sql, connection);
            return await command.ExecuteNonQueryAsync() != 1;
        }

        private Dictionary<string, int> getFieldValueMap<T>(DbDataReader reader)
             where T : Enum
        {
            List<string> columns = new List<string>();
            Dictionary<string, int> result = new Dictionary<string, int>();

            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i).ToUpper());

            foreach (string enumVal in Enum.GetNames(typeof(T)))
            {
                int index = columns.IndexOf(enumVal.ToUpper());
                result.Add(enumVal, index);
            }

            return result;
        }
    }
}
