using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace kpssOrganizerServer_TEST
{
    public static class DBManager
    {
        static SQLiteConnection connection;
        static SQLiteCommand command;

        
        public static void Connect(string dbPath)
        {
            if (!CheckFileExists(dbPath)) CreateDBFile(dbPath);

            string connString = $"Data source={dbPath}";
            connection = new SQLiteConnection(connString);
            connection.Open();
            command = new SQLiteCommand();
        }
        static bool CheckFileExists(string path)
        {
            if (File.Exists(path)) return true;
            else return false;
        }
        static void CreateDBFile(string fileName)
        {
            // Accounts, Session, Groups, Messages, ...

            SQLiteConnection.CreateFile(fileName);
            connection = new SQLiteConnection($"Data source=" + fileName);
            connection.Open();

            string accountsCreate = @"CREATE TABLE Accounts (
	id	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	username	TEXT NOT NULL UNIQUE,
	email	TEXT NOT NULL UNIQUE,
	password	TEXT NOT NULL,
	reg_date	NUMERIC NOT NULL,
	reg_ip	TEXT
);";
            command = new SQLiteCommand(accountsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created accounts table");

            string sessionsCreate = @"CREATE TABLE Sessions (
	id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	account_id	INTEGER NOT NULL UNIQUE,
	login_time	NUMERIC NOT NULL,
	FOREIGN KEY(account_id) REFERENCES Accounts(id)
);";
            command = new SQLiteCommand(sessionsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created sessions table");

            string bannedAccountsCreate = @"CREATE TABLE BannedAccounts (
	id	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	account_id	INTEGER NOT NULL UNIQUE,
	reason	TEXT,
	date	NUMERIC NOT NULL
)";
            command = new SQLiteCommand(bannedAccountsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created banned accounts table");

            string groupsCreate = @"CREATE TABLE Groups(
    id    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	login TEXT NOT NULL UNIQUE,
    password  TEXT,
	leader_id INTEGER UNIQUE
)";
            command = new SQLiteCommand(groupsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created groups table");

            connection.Close();
        }

        
        public static ResponsePacket RegisterAccount(string username, string email, string pass, string ip)
        {
            ResponsePacket packet = new ResponsePacket();
            string query = "INSERT INTO Accounts(username, email, password, reg_date, reg_ip) " +
                $"VALUES ('{username}', '{email}', '{pass}', '{DateTime.Now}', '{ip}')";
     
            command = new SQLiteCommand(query, connection);

            try
            {
                int result = command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.Message.Contains("UNIQUE"))
                {
                    if (ex.Message.Contains("Accounts.email")) packet.Code = ResponseCode.Register_Fail_EmailExists;
                    else if (ex.Message.Contains("Accounts.username")) packet.Code = ResponseCode.Register_Fail_UsernameExists;
                    return packet;
                }
                else
                {
                    packet.Code = ResponseCode.Register_Fail_Unknown;
                    return packet;
                }          
            }
            packet.Code = ResponseCode.Register_Success;

            return packet;

        }
        public static ResponsePacket LoginAccount(string email, string pass, string ip)
        {
            ResponsePacket packet = new ResponsePacket();
            string query = $"SELECT * FROM Accounts WHERE email='{email}' AND password='{pass}'";
   
            command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader;

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Console.WriteLine("asDASD");
                packet.Code = ResponseCode.Login_Fail_Unknown;
                return packet;
            }


            Console.WriteLine("not exception");
            if (reader.HasRows)
            {
                int sessionID = -1;
                Console.WriteLine("HAS ROWS");
                while (reader.Read())
                {
                    Console.WriteLine("ACC ID: " + int.Parse(reader["id"].ToString()));

                    if(CheckAccountBan(reader["id"].ToString()) == true)
                    {
                        packet.Code = ResponseCode.Login_Fail_AccoundBanned;
                        packet.Extra = GetAccountBanInfo(reader["id"].ToString());
                        return packet;
                    }

                    sessionID = StartSession(reader["id"].ToString(), ip);
                    if (sessionID == -1)
                    {                
                        packet.Code = ResponseCode.Login_Fail_SessionAlreadyExists;
                        return packet;
                    }
                }
         
                packet.Code = ResponseCode.Login_Success;
                packet.Extra = $"{sessionID}&{GetField("Accounts", "username", "email", email)}";

                return packet;

            }
            else
            {
                Console.WriteLine("incorrect data");
                packet.Code = ResponseCode.Login_Fail_IncorrectData;
                return packet;
            }

        }
        static int StartSession(string accId, string ip)
        {
            string query = $"INSERT INTO Sessions(account_id, login_time) VALUES({accId}, '{DateTime.Now}')";
            command = new SQLiteCommand(query, connection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            query = $"SELECT id FROM Sessions WHERE account_id='{accId}'";
            command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int result = -1;
            while (reader.Read())
            {
                result = int.Parse(reader["id"].ToString());
            }
            Console.WriteLine("START SESSION RETURNED " + result);
            return result;
        }
        public static List<string> GetSessions()
        {
            List<string> temp = new List<string>();
            string query = "SELECT id FROM Sessions";
          
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                temp.Add(reader["id"].ToString());
            }
      
            return temp;
        }

        public static void KillSession(string id)
        {
            string query = $"DELETE FROM Sessions WHERE id='{id}'";
         
            command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
         
        }

        public static int KillSessionList(List<string> ids)
        {
            string query = string.Empty;
         
            command = new SQLiteCommand();
            command.Connection = connection;
            foreach (var id in ids)
            {
                query = $"DELETE FROM Sessions WHERE id='{id}'";
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
  
            return ids.Count;
        }

        static string GetField(string tableName, string fieldName, string key, string value)
        {
            string query = $"SELECT {fieldName} FROM {tableName} WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();

            string result = string.Empty;
            while (reader.Read()) result = reader[fieldName].ToString();
            return result;
        }

        public static Dictionary<string, string> GetSessionDictionary()
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            string query = "SELECT id, account_id FROM Sessions";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                temp.Add(reader["id"].ToString(), reader["account_id"].ToString());
            }
            return temp;

        }

        public static void BanAccount(string key, string value, string banReason)
        {
            string getQuery = $"SELECT * FROM Accounts WHERE {key}='{value}'";
            command = new SQLiteCommand(getQuery, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string query = "INSERT INTO BannedAccounts(account_id, reason, date) " +
                    $"VALUES({reader["id"].ToString()}, '{banReason}', '{DateTime.Now}')";
                try
                {
                    command = new SQLiteCommand(query, connection);

                }
                catch(SQLiteException ex)
                {
                    Console.WriteLine("Failed to ban " + reader["username"]);
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine(reader["username"] + " UNDER BAN");
            }
        }

        public static void UnbanAccount(string key, string value)
        {
            string query = $"DELETE FROM BannedAccounts WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            Console.WriteLine($"Unbanned {command.ExecuteNonQuery()} account");
        }

        public static bool CheckAccountBan(string accId)
        {
            string query = "SELECT * FROM BannedAccounts WHERE account_id=" + accId;
            Console.WriteLine(query);
            command = new SQLiteCommand(query, connection);

            object result = command.ExecuteScalar();
            Console.WriteLine("BANNED: " + result);

            if (result!=null) return true;
            return false;

        }

        static string GetAccountBanInfo(string accId)
        {
            string query = "SELECT reason, date FROM BannedAccounts WHERE account_id=" + accId;
            string result = string.Empty;
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read()) result = $"{reader["reason"]}&{reader["date"]}";
            
            return result;
        }
    }
}
