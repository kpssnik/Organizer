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

            string groupMembersCreate = @"CREATE TABLE GroupMembers(

    id    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	group_id INTEGER NOT NULL,
	account_id    INTEGER NOT NULL,
	FOREIGN KEY(group_id) REFERENCES Groups(id),
	FOREIGN KEY(account_id) REFERENCES Accounts(id)
)";

            command = new SQLiteCommand(groupMembersCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created group members table");

            string boldedDatesCreate = @"CREATE TABLE BoldedDates(

    id    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	group_id  INTEGER NOT NULL,
	senderLogin   TEXT NOT NULL,
	description   TEXT,
	date  NUMERIC NOT NULL
); ";
            command = new SQLiteCommand(boldedDatesCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created bolded dates table");
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
        public static void ClearSessions()
        {
            string query = "DELETE FROM Sessions";
            command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
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

        static bool CheckExists(string tableName, string key, string value)
        {
            string query = $"SELECT * FROM {tableName} WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows) return true;
            else return false;
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

        public static ResponsePacket CreateGroup(string login, string pass, string leaderID)
        {
            ResponsePacket packet = new ResponsePacket();
            if(CheckExists("Groups", "login", login))
            {
                packet.Code = ResponseCode.GroupCreate_Fail_LoginExists;
                return packet;
            }
            else
            {             
                try
                {
                    string query = $"INSERT INTO Groups(login, password, leader_id) VALUES('{login}', '{pass}', {leaderID})";
                    command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();

                    query = $"SELECT id FROM Groups WHERE login='{login}' AND leader_id={leaderID}";
                    command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    int groupID = 0;
                    while (reader.Read())
                    {
                        groupID = reader.GetInt32(0);
                    }

                    query = $"INSERT INTO GroupMembers(group_id, account_id) VALUES({groupID}, {leaderID})";
                    command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    packet.Code = ResponseCode.GroupCreate_Fail_Unknown;
                    return packet;
                }
                packet.Code = ResponseCode.GroupCreate_Success;
                return packet;
            }
        }
        public static ResponsePacket JoinGroup(string login, string pass, string acc_id)
        {
            ResponsePacket packet = new ResponsePacket();
            string query = $"SELECT id FROM Groups WHERE login='{login}' AND password='{pass}'";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                packet.Code = ResponseCode.GroupJoin_Fail_IncorrectData;
                return packet;
            }
            else
            {
                
                while (reader.Read())
                {
                    query = $"SELECT * FROM GroupMembers WHERE group_id={reader.GetInt32(0)} AND account_id={acc_id}";
                    command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader2 = command.ExecuteReader();
                    while (reader2.Read())
                    {
                        if (reader2.HasRows)
                        {
                            packet.Code = ResponseCode.GroupJoin_Fail_AlreadyJoined;
                            return packet;
                        }
                    }

                    query = $"INSERT INTO GroupMembers(group_id, account_id) VALUES({reader.GetInt32(0)}, {acc_id})";
                    command = new SQLiteCommand(query, connection);
                    if(command.ExecuteNonQuery() == 1) packet.Code = ResponseCode.GroupJoin_Success;
                }
            }
            return packet;
        }
        
        public static ResponsePacket GetAccountGroups(string acc_id)
        {
            ResponsePacket packet = new ResponsePacket();
            packet.Code = ResponseCode.Default;

            List<int> groupIds = new List<int>();
            string query = $"SELECT group_id FROM GroupMembers WHERE account_id={acc_id}";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                groupIds.Add(reader.GetInt32(0));
            }
            Console.WriteLine(groupIds.Count);

            query = "SELECT login FROM Groups WHERE ";
            for(int i = 0; i < groupIds.Count; i++)
            {
                if (i == groupIds.Count - 1) query += $"id={groupIds[i]}";
                else query += $"id={groupIds[i]} OR ";
            }
            Console.WriteLine(query);

            try
            {
                 command = new SQLiteCommand(query, connection);
                 reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                packet.Extra = "";
                return packet;
            }

            while (reader.Read())
            {
                packet.Extra += reader.GetString(0)+"&";
            }
            packet.Extra.Trim('&');
            Console.WriteLine("EXTRA: " + packet.Extra);

            return packet;
        }
        public static ResponsePacket GetGroupInfo(string acc_id, string groupName)
        {
            string users = string.Empty;
            string boldedDates = string.Empty;
            string events = string.Empty;

            string groupId = GetField("Groups", "id", "login", groupName);

             
            string exists = "SELECT account_id FROM GroupMembers WHERE group_id=" + groupId;
            Console.WriteLine("QUERY: " + exists);
            command = new SQLiteCommand(exists, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            bool ex = false;
            while (reader.Read())
            {
                if (reader.GetInt32(0).ToString() == acc_id) ex = true;
            }
            ResponsePacket packet = new ResponsePacket();
            if(ex)
            {
                Console.WriteLine("ExistsExistsExistsExistsExistsExists");

                // Users
                string query = $"SELECT account_id FROM GroupMembers WHERE group_id={groupId}";
                command = new SQLiteCommand(query, connection);

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                   users += GetField("Accounts", "username", "id", reader.GetInt32(0).ToString()) + "&";
                }
                users = users.TrimEnd('&');
                Console.WriteLine("USERS: " + users);




                // BoldedDates

                query = $"SELECT date, description FROM BoldedDates WHERE group_id={groupId}";
                command = new SQLiteCommand(query, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //login^description^date&login^descr^date&...
                    boldedDates += $"{reader.GetString(0)}^{reader.GetString(1)}&";
                }
                boldedDates = boldedDates.TrimEnd('&');
                Console.WriteLine("BOLDED DATES: " + boldedDates);




                // Events

                query = $"SELECT event_text FROM GroupEvents WHERE group_id={groupId} ORDER BY id DESC LIMIT 25";
                command = new SQLiteCommand(query, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    events += reader.GetString(0) + "&";
                }
                events = events.TrimEnd('&');
                Console.WriteLine("EVENTS: " + events);


                packet.Code = ResponseCode.GroupInfo;
                packet.Extra = $"{users}\n{boldedDates}\n{events}";
            }
            else
            {
                packet.Code = ResponseCode.GroupInfo;
                packet.Extra = "";
            }
                return packet;
        }

        public static void BoldDate(string groupName, string login, string date, string description)
        {
            Console.WriteLine("GROUP NAME: " + groupName);
            string groupId = GetField("Groups", "id", "login", groupName);
            Console.WriteLine("GROUP ID: " + groupId);


            string query = "INSERT INTO BoldedDates(group_id, senderLogin, description, date) VALUES" +
                $"({groupId}, '{login}', '{description}', '{date}')";
            Console.WriteLine("BOLD DATE QUERY: " + query);

            
            command = new SQLiteCommand(query, connection);
            if(command.ExecuteNonQuery() > 0)
            {
                query = "INSERT INTO GroupEvents(group_id, event_text, date) VALUES" +
                    $"({groupId}, '{login} bolded {date}. Description: {description}', '{DateTime.Now.ToString()}')";

                command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }

            
        }

        public static void DeleteBoldedDate(string groupName, string date)
        {

        }
    }
}
