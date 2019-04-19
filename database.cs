using Discord;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace tobybot
{
    public class Database
    {
        // Using databases for owner customization will be held off for now. Had some technical issues.
        private string table { get; set; }
        private const string server = "1";
        private const string database = "2";
        private const string username = "3";
        private const string password = "4";
        private MySqlConnection dbConnection;

        public Database(string table)
        {
            this.table = table;
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Server = server;
            stringBuilder.UserID = username;
            stringBuilder.Password = password;
            stringBuilder.Database = database;
            stringBuilder.Port = 3306;
            stringBuilder.SslMode = MySqlSslMode.None;

            var connectionString = stringBuilder.ToString();

            dbConnection = new MySqlConnection(connectionString);

            dbConnection.Open();
        }

        public MySqlDataReader FireCommand(string query)
        {
            if (dbConnection == null)
            {
                return null;
            }

            MySqlCommand command = new MySqlCommand(query, dbConnection);

            var mySqlReader = command.ExecuteReader();

            return mySqlReader;
        }

        public void CloseConnection()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
            }
        }

        public static string EnterServer(SocketGuild guild)
        {
            var database = new Database("server_setup");

            var str = string.Format("INSERT INTO server_setup (guild_id) VALUES ('{0}')", guild.Id);
            var table = database.FireCommand(str);

            database.CloseConnection();

            return null;
        }

        public static List<int> GetServerConfig(SocketGuild guild, string config)
        {
            var result = new List<int>();

            var database = new Database("config");

            var str = string.Format("SELECT * FROM server_setup WHERE guild_id = '{0}'", guild.Id);
            var tableName = database.FireCommand(str);

            while (tableName.Read())
            {
                var configs = (int)tableName[config];

                result.Add(configs);
            }
            database.CloseConnection();

            return result;

        }
        public static void ChangeWelcomeChannel(SocketGuild guild, string channelId)
        {
            var database = new Database("server_setup");

            try
            {
                var strings = string.Format("UPDATE server_setup SET welcome_id = '{1}' WHERE guild_id = '{0}'", guild.Id, channelId);
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception e)
            {
                database.CloseConnection();
                Console.WriteLine(e.ToString());
                return;
            }
        }
        public static List<Server> CheckExistingServer(SocketGuild guild)
        {
            var result = new List<Server>();

            var database = new Database("config");

            var str = string.Format("SELECT * FROM server_setup WHERE guild_id = '{0}'", guild.Id);
            var tableName = database.FireCommand(str);

            while (tableName.Read())
            {
                var serverId = (string)tableName["guild_id"];
                var welcomeId = (string)tableName["welcome_id"];
                var roleId = (string)tableName["role_id"];

                result.Add(new Server
                {
                    ServerId = serverId,
                    WelcomeId = welcomeId,
                    RoleId = roleId
                });
            }
            database.CloseConnection();

            return result;

        }
    }
}