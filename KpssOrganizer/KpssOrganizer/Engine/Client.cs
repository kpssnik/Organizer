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
    class Client
    {
        UdpClient client = new UdpClient();

        public string sessionLogin = string.Empty;
        public string sessionID = string.Empty;
        public string responseString = string.Empty;

        private string serverIp = "127.0.0.1";

        public Client()
        {

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
            Thread sessionKeeper = new Thread(() =>
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
            sessionKeeper.Start();
            Console.WriteLine("Started holding");
        }

        public ResponseCode GetResponseCode(string str)
        {
            return (ResponseCode)int.Parse(str);
        }
        private PacketType GetPacketType(string str)
        {
            return (PacketType)int.Parse(str);
        }

    }
}
