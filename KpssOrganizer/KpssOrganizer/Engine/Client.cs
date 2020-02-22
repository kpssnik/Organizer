using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KpssOrganizer.Engine;


namespace KpssOrganizer
{
    class Client:IDisposable
    {
        UdpClient client = new UdpClient();

        public string sessionLogin = string.Empty;
        public string sessionID = string.Empty;
        public string responseString = string.Empty;

        Thread sessionKeeper;

        public bool canActive = true;

        private string serverIp = "127.0.0.1";

        public Client()
        {
            sessionKeeper = new Thread(() =>
            {
                using (UdpClient sender = new UdpClient(serverIp, (int)Port.Server_SessionCheck))
                {
                    while (true)
                    {
                        Thread.Sleep(20000);
                        SessionContinuePacket packet = new SessionContinuePacket()
                        {
                            SessionID = sessionID
                        };
                        Console.WriteLine(Crypto.SimEncrypt(packet.BuildPacket()));
                        byte[] data = Crypto.SimEncrypt(packet.BuildPacket());

                        sender.Send(data, data.Length);
                    }
                }
            });
        }
        public void Connect(int port)
        {
            if (client.Client.Connected) client.Client.Close();
            client = new UdpClient(serverIp, port);
            Console.WriteLine($"Connected to {client.Client.RemoteEndPoint.ToString()}");
        }
        private void SendPacket(byte[] data)
        {
            int sentBytes = client.Send(data, data.Length);
            Console.WriteLine($"Sent {sentBytes} bytes");
        }

        public ResponseCode Register(string name, string email, string pass)
        {
            RegisterPacket packet = new RegisterPacket()
            {
                Username = name,
                Email = email,
                Password = pass
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));
            ResponseCode responseCode = GetResponseCode(ReceiveResponse(receiver).Split('%')[1]);
            receiver.Close();

            return responseCode;         
        }

        public ResponseCode Login(string email, string pass)
        {
            LoginPacket packet = new LoginPacket()
            {
                Email = email,
                Password = pass
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));

            string[] response = ReceiveResponse(receiver).Split('%');
            receiver.Close();

            switch (GetResponseCode(response[1]))
            {
                case ResponseCode.Login_Success:
                    sessionID = response[2].Split('&')[0];
                    sessionLogin = response[2].Split('&')[1];
                    return ResponseCode.Login_Success;

                case ResponseCode.Login_Fail_AccoundBanned:
                    string banReason = response[2].Split('&')[0];
                    string banDate = response[2].Split('&')[1];
                    responseString = $"Login error. Account banned.\nReason: {banReason}\nDate:{banDate}";
                    return ResponseCode.Login_Fail_AccoundBanned;

                default:
                    return (GetResponseCode(response[1]));
            }
        }

        private string ReceiveResponse(UdpClient receiver)
        {
            byte[] response = new byte[] { };
            IPEndPoint ip = null;

            response = receiver.Receive(ref ip);

            string[] decrypt = Crypto.SimDecrypt(response).Split('%');
            if (GetPacketType(decrypt[0]) == PacketType.Response)
            {
                return Crypto.SimDecrypt(response);
            }
            else throw new Exception("Invalid packet");

        }

        public void HoldSession()
        {
            sessionKeeper.Start();
        }

        public ResponseCode CreateGroup(string login, string password = "")
        {
            GroupCreatePacket packet = new GroupCreatePacket()
            {
                Login = login,
                Password = password,
                SessionID = sessionID,
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));

            ResponseCode responseCode = GetResponseCode(ReceiveResponse(receiver).Split('%')[1]);
            receiver.Close();

            return responseCode;           
        }

        public ResponseCode JoinGroup(string login, string password = "")
        {
            GroupJoinPacket packet = new GroupJoinPacket()
            {
                Login = login,
                Password = password,
                SessionID = sessionID
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));

            ResponseCode responseCode = GetResponseCode(ReceiveResponse(receiver).Split('%')[1]);
            receiver.Close();

            return responseCode;
        }

        public List<string> GetGroupsList()
        {
            List<string> temp = new List<string>();
            GetGroupsListPacket packet = new GetGroupsListPacket()
            {
                SessionID = sessionID
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));

            string extra = ReceiveResponse(receiver).Split('%')[2].TrimEnd('&');
            receiver.Close();

            foreach(var a in extra.Split('&')) temp.Add(a);
           
            return temp;
        }

        public string GetGroupInfo(string groupName)
        {
            GetGroupInfoPacket packet = new GetGroupInfoPacket()
            {
                SessionID = sessionID,
                GroupName = groupName
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));

            string response = ReceiveResponse(receiver).Split('%')[2];
            //string users = response.Split('%')[2].TrimEnd('&');

            receiver.Close();

            return response;
        }

        public void DoWait(int ms)
        {
            new Thread(() =>
            {
                canActive = false;
                Thread.Sleep(ms);
                canActive = true;
            }).Start();
        }

        public void SendBoldedDate(string date, string description, string groupName)
        {
            BoldDatePacket packet = new BoldDatePacket()
            {
                Date = date,
                Description = description,
                GroupName = groupName,
                Login = sessionLogin,
            };

            UdpClient receiver = new UdpClient((int)Port.Client_ResponseReceive);
            SendPacket(Crypto.SimEncrypt(packet.BuildPacket()));
            receiver.Close();
        }


        public ResponseCode GetResponseCode(string str)
        {
            return (ResponseCode)int.Parse(str);
        }
        private PacketType GetPacketType(string str)
        {
            return (PacketType)int.Parse(str);
        }

        public void Dispose()
        {
            sessionKeeper.Abort();
        }
    }
}
