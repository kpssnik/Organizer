using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using KpssOrganizerServer.Database;

namespace KpssOrganizerServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartInputThread();

        }
    }

    public class Server
    {
        string dbPath = "../../Database/database.sqlite";
        List<string> deadSessionIds = new List<string>();
        Dictionary<string, string> onlineUsers { get { return DBManager.GetSessionDictionary(); } }

        public List<Thread> listeningThreads = new List<Thread>();

        public Server()
        {

        }

        public void DoListen(int port)
        {
            UdpClient listener = new UdpClient(port);
            IPEndPoint remoteIp = null;

            Thread th = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] data = listener.Receive(ref remoteIp);
                        string message = Crypto.SimDecrypt(data);

                        new Thread(new ParameterizedThreadStart(HandlePacket)).Start(message);
                    }
                    catch (Exception ex)
                    {
                        PrintMessage(ex.Message, ConsoleColor.Red);
                    }
                }
            });
            th.Start();
            listeningThreads.Add(th);
        }

        public void HandlePacket(object packet)
        {
            string[] packetInfo = packet.ToString().Split('%');

            switch (GetPacketType(packetInfo[0]))
            {
                case PacketType.Login:
                    ResponsePacket loginResponsePacket = DBManager.LoginAccount(packetInfo[1], packetInfo[2], packetInfo[3]);
                    SendResponse(loginResponsePacket, packetInfo[3]);
                    break;

                case PacketType.Register:
                    ResponsePacket registerResponsePacket = DBManager.RegisterAccount(packetInfo[1], packetInfo[2], packetInfo[3], packetInfo[4]);
                    SendResponse(registerResponsePacket, packetInfo[4]);
                    break;

                case PacketType.SessionContinue:
                    ContinueSession(packetInfo[1]);
                    break;

                case PacketType.GroupCreate:
                    ResponsePacket groupCreateResponsePacket = DBManager.CreateGroup(packetInfo[1], packetInfo[2], onlineUsers[packetInfo[3]]);
                    SendResponse(groupCreateResponsePacket, packetInfo[4]);
                    break;

                case PacketType.GroupJoin:
                    ResponsePacket groupJoinResponsePacket = DBManager.JoinGroup(packetInfo[1], packetInfo[2], onlineUsers[packetInfo[3]]);
                    SendResponse(groupJoinResponsePacket, packetInfo[4]);
                    break;

                case PacketType.GetGroupsList:
                    ResponsePacket groupsListPacket = DBManager.GetAccountGroups(onlineUsers[packetInfo[1]]);
                    SendResponse(groupsListPacket, packetInfo[2]);
                    break;

                case PacketType.GetGroupInfo:
                    ResponsePacket groupInfoPacket = DBManager.GetGroupInfo(onlineUsers[packetInfo[2]], packetInfo[1]);
                    SendResponse(groupInfoPacket, packetInfo[3]);
                    break;

                case PacketType.BoldDate:
                    DBManager.BoldDate(packetInfo[1], packetInfo[2], packetInfo[3], packetInfo[4]);
                    break;

                case PacketType.DeleteBoldedDate:
                    DBManager.DeleteBoldedDate(packetInfo[1], packetInfo[2], packetInfo[3]);
                    break;

            }
        }

        public void SendResponse(ResponsePacket packet, string toIp)
        {
            UdpClient sender = new UdpClient(toIp, (int)Port.Client_ResponseReceive);

            byte[] data = Crypto.SimEncrypt(packet.BuildPacket());
            sender.Send(data, data.Length);

            sender.Close();
        }

        public void DoCheckSessions()
        {
            DBManager.KillSessionList(DBManager.GetSessions());
            Thread checker = new Thread(() =>
            {
                while (true)
                {
                    deadSessionIds = DBManager.GetSessions();
                    Thread.Sleep(40000);
                    DBManager.KillSessionList(deadSessionIds);
                    Console.WriteLine($"Killed {deadSessionIds.Count} sessions");
                }

            });
            checker.Start();
        }

        public void ContinueSession(string sessionID)
        {
            if (deadSessionIds.Contains(sessionID)) deadSessionIds.Remove(sessionID);
        }

        private PacketType GetPacketType(string str)
        {
            return (PacketType)int.Parse(str);
        }

        public void StartInputThread()
        {
            new Thread(() =>
            {
                while (true)
                {
                    string[] command = Console.ReadLine().Split(' ');

                    try
                    {
                        switch (command[0])
                        {
                            case "/start":
                                DBManager.Connect(dbPath);
                                DoCheckSessions();
                                DBManager.ClearSessions();
                                DoListen((int)Port.Server_LoginRegister);
                                DoListen((int)Port.Server_SessionCheck);
                                DoListen((int)Port.Server_MainReceiver);

                                PrintMessage("Server started with 3 listening threads", ConsoleColor.Green);
                                break;

                            case "/stop":
                                //foreach (var th in listeningThreads) th.Abort();
                                Environment.Exit(0);
                                break;

                            case "/clear":
                                Console.Clear();
                                PrintMessage("Console cleared", ConsoleColor.Green);
                                break;

                            case "/ban":
                                DBManager.BanAccount(command[1], command[2], command[3]);
                                break;

                            case "/unban":
                                DBManager.UnbanAccount(command[1], command[2]);
                                break;

                            case "/get":
                                switch (command[1])
                                {
                                    case "ai":
                                        DBManager.PrintAccountInfo(command[2], command[3]);
                                        break;

                                    case "bi":
                                        DBManager.PrintBanInfo(command[2], command[3]);
                                        break;

                                    case "gi":
                                        DBManager.PrintGroup(command[2], command[3]);
                                        break;

                                }

                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        PrintMessage(ex.Message, ConsoleColor.Red);
                    }
                }

            }).Start();
        }

        public static void PrintMessage(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

    }

}
