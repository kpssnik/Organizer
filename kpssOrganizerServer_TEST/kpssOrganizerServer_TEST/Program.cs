using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace kpssOrganizerServer_TEST
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.DoListen((int)Port.Server_LoginRegister);
            server.DoListen((int)Port.Server_SessionCheck);
            server.DoListen((int)Port.Server_MainReceiver);

            //DBManager.BanAccount("username", "ggwpFCVP", "pidoras");
        }


    }

    public class Server
    {
        string dbPath = "../../../Database/database.sqlite";
        List<string> deadSessionIds = new List<string>();
        Dictionary<string, string> onlineUsers { get { return DBManager.GetSessionDictionary(); } }

        public Server()
        {
            DBManager.Connect(dbPath);
            DoCheckSessions();
            DBManager.ClearSessions();
        }

        public void DoListen(int port)
        {
            UdpClient listener = new UdpClient(port);
            IPEndPoint remoteIp = null;

            Thread th = new Thread(() =>
            {
                while (true)
                {
                    byte[] data = listener.Receive(ref remoteIp);
                    string message = Crypto.SimDecrypt(data);

                    new Thread(new ParameterizedThreadStart(HandlePacket)).Start(message);
                }
            });
            th.Start();
        }


        public void HandlePacket(object packet)
        {
            string[] packetInfo = packet.ToString().Split('%');

            switch (GetPacketType(packetInfo[0]))
            {
                case PacketType.Login:
                    Console.WriteLine("\nINCOMING LOGIN PACKET");
                    Console.WriteLine($"Email: {packetInfo[1]}\nPass: {packetInfo[2]}\nIP: {packetInfo[3]}\n\n");

                    ResponsePacket loginResponsePacket = DBManager.LoginAccount(packetInfo[1], packetInfo[2], packetInfo[3]);
                    Console.WriteLine("TUT: " + loginResponsePacket.Code);
                    SendResponse(loginResponsePacket, packetInfo[3]);                  
                    break;

                case PacketType.Register:
                    Console.WriteLine("\nINCOMING REGISTER PACKET");
                    Console.WriteLine($"Username: {packetInfo[1]}\nEmail: {packetInfo[2]}\nPass: {packetInfo[3]}\nIP: {packetInfo[4]}\n\n");

                    ResponsePacket registerResponsePacket = DBManager.RegisterAccount(packetInfo[1], packetInfo[2], packetInfo[3], packetInfo[4]);
                    SendResponse(registerResponsePacket, packetInfo[4]);
                    break;

                case PacketType.SessionContinue:
                    ContinueSession(packetInfo[1]);
                    Console.WriteLine(packetInfo[1] + " session continued.\n");
                    break;

                case PacketType.GroupCreate:
                    Console.WriteLine("INCOMING GROUP CREATE PACKET");
                    Console.WriteLine($"{packetInfo[1]}\t{packetInfo[2]}\t{packetInfo[3]}");

                    ResponsePacket groupCreateResponsePacket = DBManager.CreateGroup(packetInfo[1], packetInfo[2], onlineUsers[packetInfo[3]]);
                    Console.WriteLine($"PACKET___________\t" + $"{groupCreateResponsePacket.Code}({(int)groupCreateResponsePacket.Code})");
                    SendResponse(groupCreateResponsePacket, packetInfo[4]);
                    break;

                case PacketType.GroupJoin:
                    Console.WriteLine("INCOMING GROUP JOIN PACKET");
                    Console.WriteLine($"{packetInfo[1]}\t{packetInfo[2]}\t{packetInfo[3]}");

                    ResponsePacket groupJoinResponsePacket = DBManager.JoinGroup(packetInfo[1], packetInfo[2], onlineUsers[packetInfo[3]]);
                    SendResponse(groupJoinResponsePacket, packetInfo[4]);
                    break;

                case PacketType.GetGroupsList:
                    Console.WriteLine("INCOMING GET GROUPS PACKET");
                    Console.WriteLine($"{packetInfo[1]}\t{packetInfo[2]}");

                    ResponsePacket groupsListPacket = DBManager.GetAccountGroups(onlineUsers[packetInfo[1]]);
                    Console.WriteLine("\t\tSENDING RESPONSE: " + groupsListPacket.Extra);
                    SendResponse(groupsListPacket, packetInfo[2]);
                    break;

                case PacketType.GetGroupInfo:
                    Console.WriteLine("INCOMING GET GROUP INFO PACKET");
                    Console.WriteLine($"{packetInfo[1]}\t{packetInfo[2]}\t{packetInfo[3]}");

                    ResponsePacket groupInfoPacket = DBManager.GetGroupInfo(onlineUsers[packetInfo[2]], packetInfo[1]);
                    SendResponse(groupInfoPacket, packetInfo[3]);
                    break;

                case PacketType.BoldDate:
                    Console.WriteLine("INCOMING BOLD DATE PACKET");
                    Console.WriteLine($"{packetInfo[1]}\t{packetInfo[2]}\t{packetInfo[3]}\t{packetInfo[4]}");

                    DBManager.BoldDate(packetInfo[1], packetInfo[2], packetInfo[3], packetInfo[4]);
                    break;
            }
        }

        public void SendResponse(ResponsePacket packet, string toIp)
        {
            UdpClient sender = new UdpClient(toIp, (int)Port.Client_ResponseReceive);
           
            byte[] data = Crypto.SimEncrypt(packet.BuildPacket());
            sender.Send(data, data.Length);

            sender.Close();

            foreach(var a in onlineUsers)
            {
                Console.WriteLine(a.Key + " => " + a.Value);
            }
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
            if(deadSessionIds.Contains(sessionID)) deadSessionIds.Remove(sessionID);
        }

        private PacketType GetPacketType(string str)
        {
            return (PacketType)int.Parse(str);
        }

    }


}

