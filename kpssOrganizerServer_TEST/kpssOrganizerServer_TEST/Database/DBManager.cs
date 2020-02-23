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
            Server.PrintMessage("Creating db file and tables...", ConsoleColor.White);
            // Accounts, Session, Groups, Messages, ...

            SQLiteConnection.CreateFile(fileName);
            connection = new SQLiteConnection($"Data source=" + fileName);
            connection.Open();

            string accountsCreate = @"CREATE TABLE Accounts (
	id	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	username	TEXT NOT NULL UNIQUE,
	email	TEXT NOT NULL UNIQUE,
	password	TEXT NOT NULL,
	reg_date	TEXT NOT NULL,
	reg_ip	TEXT
);";
            command = new SQLiteCommand(accountsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created accounts table");

            string sessionsCreate = @"CREATE TABLE Sessions (
	id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	account_id	INTEGER NOT NULL UNIQUE,
	login_time	TEXT NOT NULL,
	FOREIGN KEY(account_id) REFERENCES Accounts(id)
);";
            command = new SQLiteCommand(sessionsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created sessions table");

            string bannedAccountsCreate = @"CREATE TABLE BannedAccounts (
	id	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	account_id	INTEGER NOT NULL UNIQUE,
	reason	TEXT,
	date	TEXT NOT NULL
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
	date  TEXT NOT NULL
); ";
            command = new SQLiteCommand(boldedDatesCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created bolded dates table");

            string groupEventsCreate = @"CREATE TABLE GroupEvents(

    id    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	group_id  INTEGER NOT NULL,
	event_text    TEXT NOT NULL,
	date  TEXT NOT NULL,
	FOREIGN KEY(group_id) REFERENCES Groups(id)
); ";
            command = new SQLiteCommand(groupEventsCreate, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Created group events table");

            connection.Close();
        }

        public static void PrintAccountInfo(string key, string value)
        {
            string query = $"SELECT * FROM Accounts WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Server.PrintMessage($"ID: {reader["id"]} \nLogin: {reader["username"]} \nEmail: {reader["email"]} \nReg. date: " +
                    $"{reader["reg_date"]} \nReg. ip: {reader["reg_ip"]}\n\n", ConsoleColor.White);
            }
        }

        public static void PrintBanInfo(string key, string value)
        {
            string query = $"SELECT * FROM BannedAccounts WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Server.PrintMessage($"ID: {reader["id"]} \nAccount id: {reader["account_id"]} \nReason: {reader["reason"]} \nDate: " +
                    $"{reader["date"]}\n\n", ConsoleColor.White);
            }
        }

        public static void PrintGroup(string key, string value)
        {
            string query = $"SELECT * FROM Groups WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Server.PrintMessage($"ID: {reader["id"]} \nLogin: {reader["login"]} \nPassword: {reader["password"]} \nLeader id: " +
                    $"{reader["leader_id"]}\n\n", ConsoleColor.White);
            }
        }

        

        public static ResponsePacket RegisterAccount(string username, string email, string pass, string ip)
        {
            ResponsePacket packet = new ResponsePacket();
            string query = "INSERT INTO Accounts(username, email, password, reg_date, reg_ip) " +
                $"VALUES ('{username}', '{email}', '{pass}', '{DateTime.Now.ToString()}', '{ip}')";

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
                    Server.PrintMessage($"REGISTER ERROR FROM {username}({ip})\n{ex.Message}", ConsoleColor.Red);
                    packet.Code = ResponseCode.Register_Fail_Unknown;
                    return packet;
                }
            }
            Server.PrintMessage($"{username} registered with email {email} FROM {ip}", ConsoleColor.White);
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
                Server.PrintMessage($"Login error from {email}({ip})", ConsoleColor.Red);
                Server.PrintMessage(ex.Message, ConsoleColor.Red);
                packet.Code = ResponseCode.Login_Fail_Unknown;
                return packet;
            }

            try
            {


                if (reader.HasRows)
                {
                    int sessionID = -1;
                    while (reader.Read())
                    {
                        if (CheckAccountBan(reader["id"].ToString()) == true)
                        {
                            Server.PrintMessage($"{reader["username"]} failed to login. Account banned.", ConsoleColor.Yellow);
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
                    string username = GetField("Accounts", "username", "email", email);
                    packet.Extra = $"{sessionID}&{username}";
                    Server.PrintMessage($"{username} logged in. Session id: {sessionID}", ConsoleColor.White);

                    return packet;

                }
                else
                {
                    Server.PrintMessage($"Login fail from {email}({ip}). Incorrect data", ConsoleColor.Yellow);
                    packet.Code = ResponseCode.Login_Fail_IncorrectData;
                    return packet;
                }
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR LOGIN FROM {reader["username"]}\n{ex.Message}", ConsoleColor.Red);
                packet.Code = ResponseCode.Login_Fail_Unknown;
                return packet;
            }
        }
        static int StartSession(string accId, string ip)
        {
            string query = $"INSERT INTO Sessions(account_id, login_time) VALUES({accId}, '{DateTime.Now.ToString()}')";
            command = new SQLiteCommand(query, connection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"START SESSION ERROR FOR accid {accId}({ip})\n" + ex.Message, ConsoleColor.Red);
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

            try
            {
                command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR KILL SESSION {id}.\n{ex.Message}", ConsoleColor.Red);
            }


        }
        public static int KillSessionList(List<string> ids)
        {
            string query = string.Empty;

            command = new SQLiteCommand();
            command.Connection = connection;
            try
            {

                foreach (var id in ids)
                {
                    query = $"DELETE FROM Sessions WHERE id='{id}'";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR KILL SESSION LIST.\n{ex.Message}", ConsoleColor.Red);
                return 0;
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
            try
            {
                SQLiteDataReader reader = command.ExecuteReader();

                string result = string.Empty;
                while (reader.Read()) result = reader[fieldName].ToString();
                return result;
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR GET FIELD {fieldName} FROM {tableName}. KEY: {key} VALUE: {value}\n{ex.Message}", ConsoleColor.Red);
                return string.Empty;
            }
        }

        static bool CheckExists(string tableName, string key, string value)
        {
            string query = $"SELECT * FROM {tableName} WHERE {key}='{value}'";
            command = new SQLiteCommand(query, connection);

            try
            {
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows) return true;
                else return false;
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR CHECK EXISTS FROM {tableName}. KEY: {key} VALUE: {value}.\n{ex.Message}", ConsoleColor.Red);
                return false;
            }
        }
        public static Dictionary<string, string> GetSessionDictionary()
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            string query = "SELECT id, account_id FROM Sessions";
            command = new SQLiteCommand(query, connection);

            try
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    temp.Add(reader["id"].ToString(), reader["account_id"].ToString());
                }
                return temp;
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR GET SESSION DICTIONARY.\n{ex.Message}", ConsoleColor.Red);
                return new Dictionary<string, string>();
            }

        }
        public static void BanAccount(string key, string value, string banReason)
        {
            string getQuery = $"SELECT * FROM Accounts WHERE {key}='{value}'";
            command = new SQLiteCommand(getQuery, connection);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string query = "INSERT INTO BannedAccounts(account_id, reason, date) " +
                    $"VALUES({reader["id"].ToString()}, '{banReason}', '{DateTime.Now.ToString()}')";
                try
                {
                    command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Server.PrintMessage("Failed to ban " + reader["username"] + "\n" + ex.Message, ConsoleColor.Red);
                    return;
                }

                Server.PrintMessage($"{reader["username"]} under ban. Reason: {banReason}", ConsoleColor.Green);
            }
        }
        public static void UnbanAccount(string key, string value)
        {
            string query = $"DELETE FROM BannedAccounts WHERE {key}='{value}'";
            try
            {
                command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"BAN {value} ERROR\n" + ex.Message, ConsoleColor.Red);
                return;
            }

            Server.PrintMessage($"Sucessfully unbanned {value}", ConsoleColor.Green);
        }
        public static bool CheckAccountBan(string accId)
        {
            string query = "SELECT * FROM BannedAccounts WHERE account_id=" + accId;
            command = new SQLiteCommand(query, connection);
            try
            {
                object result = command.ExecuteScalar();

                if (result != null) return true;
                return false;
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR CHECKING BAN OF {accId}\n{ex.Message}", ConsoleColor.Red);
                return false;
            }

        }
        static string GetAccountBanInfo(string accId)
        {
            string query = "SELECT reason, date FROM BannedAccounts WHERE account_id=" + accId;
            string result = string.Empty;
            command = new SQLiteCommand(query, connection);

            try
            {

                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read()) result = $"{reader["reason"]}&{reader["date"]}";

            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR GET BAN INFO of {accId}.\n{ex.Message}", ConsoleColor.Red);
                return "";
            }
            return result;
        }

        public static ResponsePacket CreateGroup(string login, string pass, string leaderID)
        {
            ResponsePacket packet = new ResponsePacket();
            if (CheckExists("Groups", "login", login))
            {
                Server.PrintMessage($"{leaderID} id failed to create group {login}. Login already exists", ConsoleColor.Yellow);
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
                    Server.PrintMessage($"{leaderID} id ERROR CREATE GROUP\n{ex.Message}", ConsoleColor.Red);
                    packet.Code = ResponseCode.GroupCreate_Fail_Unknown;
                    return packet;
                }
                Server.PrintMessage($"{leaderID} id has created group {login}", ConsoleColor.White);
                packet.Code = ResponseCode.GroupCreate_Success;
                return packet;
            }
        }
        public static ResponsePacket JoinGroup(string login, string pass, string acc_id)
        {
            ResponsePacket packet = new ResponsePacket();
            string query = $"SELECT id FROM Groups WHERE login='{login}' AND password='{pass}'";
            command = new SQLiteCommand(query, connection);

            try
            {


                SQLiteDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    Server.PrintMessage($"{acc_id} failed to join {login} group. Incorrect data", ConsoleColor.Yellow);
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
                                Server.PrintMessage($"{acc_id} failed to join {login} group. Already joined", ConsoleColor.Yellow);
                                packet.Code = ResponseCode.GroupJoin_Fail_AlreadyJoined;
                                return packet;
                            }
                        }

                        query = $"INSERT INTO GroupMembers(group_id, account_id) VALUES({reader.GetInt32(0)}, {acc_id})";
                        command = new SQLiteCommand(query, connection);
                        if (command.ExecuteNonQuery() == 1)
                        {
                            packet.Code = ResponseCode.GroupJoin_Success;
                            Console.WriteLine($"{acc_id} id has joined to {login} group");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR JOINING to {login} FROM {acc_id}\n{ex.Message}", ConsoleColor.Red);
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

            try
            {


                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    groupIds.Add(reader.GetInt32(0));
                }

                query = "SELECT login FROM Groups WHERE ";
                for (int i = 0; i < groupIds.Count; i++)
                {
                    if (i == groupIds.Count - 1) query += $"id={groupIds[i]}";
                    else query += $"id={groupIds[i]} OR ";
                }

                try
                {
                    command = new SQLiteCommand(query, connection);
                    reader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    packet.Extra = "";
                    return packet;
                }

                while (reader.Read())
                {
                    packet.Extra += reader.GetString(0) + "&";
                }
                packet.Extra = packet.Extra.Trim('&');

            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR GET ACCOUNT GROUPS FROM {acc_id}\n{ex.Message}", ConsoleColor.Red);
            }
            return packet;
        }
        public static ResponsePacket GetGroupInfo(string acc_id, string groupName)
        {
            string users = string.Empty;
            string boldedDates = string.Empty;
            string events = string.Empty;
            ResponsePacket packet = new ResponsePacket();
            try
            {
                string groupId = GetField("Groups", "id", "login", groupName);

                string exists = "SELECT account_id FROM GroupMembers WHERE group_id=" + groupId;
                command = new SQLiteCommand(exists, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                bool ex = false;
                while (reader.Read())
                {
                    if (reader.GetInt32(0).ToString() == acc_id) ex = true;
                }

                if (ex)
                {
                    // Users
                    string query = $"SELECT account_id FROM GroupMembers WHERE group_id={groupId}";
                    command = new SQLiteCommand(query, connection);

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        users += GetField("Accounts", "username", "id", reader.GetInt32(0).ToString()) + "&";
                    }
                    users = users.TrimEnd('&');




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




                    // Events

                    query = $"SELECT event_text FROM GroupEvents WHERE group_id={groupId} ORDER BY id DESC LIMIT 25";
                    command = new SQLiteCommand(query, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        events += reader.GetString(0) + "&";
                    }
                    events = events.TrimEnd('&');


                    packet.Code = ResponseCode.GroupInfo;
                    packet.Extra = $"{users}\n{boldedDates}\n{events}";
                }
                else
                {
                    packet.Code = ResponseCode.GroupInfo;
                    packet.Extra = "";
                }
            }
            catch (Exception ex)
            {
                Server.PrintMessage($"ERROR GET GROUP INFO FROM {acc_id} of {groupName}\n{ex.Message}", ConsoleColor.Red);
            }
            return packet;
        }

        public static void BoldDate(string groupName, string login, string date, string description)
        {
            string groupId = GetField("Groups", "id", "login", groupName);

            string query = "INSERT INTO BoldedDates(group_id, senderLogin, description, date) VALUES" +
                $"({groupId}, '{login}', '{description}', '{date}')";

            command = new SQLiteCommand(query, connection);
            if (command.ExecuteNonQuery() > 0)
            {
                query = "INSERT INTO GroupEvents(group_id, event_text, date) VALUES" +
                    $"({groupId}, '{login} bolded {date}. Description: {description}', '{DateTime.Now.ToString()}')";

                command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
            }


        }

        public static void DeleteBoldedDate(string groupName, string date, string login)
        {
            string groupId = GetField("Groups", "id", "login", groupName);

            string query = $"DELETE FROM BoldedDates WHERE group_id={groupId} AND date='{date}'";
            string query2 = $"INSERT INTO GroupEvents(group_id, event_text, date) VALUES({groupId}, '{login} deleted " +
                $"{date}', '{DateTime.Now.ToString()}')";

            command = new SQLiteCommand(query, connection);
            try
            {
                command.ExecuteNonQuery();
                command = new SQLiteCommand(query2, connection);
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Server.PrintMessage($"ERROR DELETING BOLDED DATE ({date} from {groupName})\n{ex.Message}", ConsoleColor.Red);
            }


        }
    }
}
