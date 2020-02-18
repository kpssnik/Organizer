using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KpssOrganizer.Engine
{
    public enum PacketType
    {
        Register,
        Login,
        Message,
        Response,
        SessionContinue
    }
    public enum ResponseCode
    {
        Register_Success = 100,
        Register_Fail_Unknown = 101,
        Register_Fail_UsernameExists = 102,
        Register_Fail_EmailExists = 103,

        Login_Success = 200,
        Login_Fail_Unknown = 201,
        Login_Fail_IncorrectData = 202,
        Login_Fail_SessionAlreadyExists = 203,
        Login_Fail_AccoundBanned = 204,

        ERROR_Invalid_Packet_Income = 300
    }

    public enum Port
    {
        Server_LoginRegister = 8880,
        Server_SessionCheck = 8881,
        Client_ResponseReceive = 8890
    }
    public interface IPacket
    {
        PacketType Type { get; }
        string BuildPacket();

    }

    class RegisterPacket : IPacket
    {
        public PacketType Type { get { return PacketType.Register; } }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        string internalIP
        {
            get
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var result = from a in host.AddressList
                             where a.AddressFamily == AddressFamily.InterNetwork && a.ToString().Contains("192.168.1")
                             select a.ToString();

                return result.LastOrDefault();
            }
        }


        public string BuildPacket()
        {
            string str = $"{(int)Type}%{Username}%{Email}%{Crypto.ComputeSha256Hash(Password)}%{internalIP}";
            Console.WriteLine(str);
            return str;
        }
    }

    class LoginPacket : IPacket
    {
        public PacketType Type { get { return PacketType.Login; } }
        public string Email { get; set; }
        public string Password { get; set; }
        string internalIP
        {
            get
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var result = from a in host.AddressList
                             where a.AddressFamily == AddressFamily.InterNetwork && a.ToString().Contains("192.168.1")
                             select a.ToString();

                return result.LastOrDefault();
            }
        }

        public string BuildPacket()
        {
            string str = $"{(int)Type}%{Email}%{Crypto.ComputeSha256Hash(Password)}%{internalIP}";
            Console.WriteLine(str);
            return str;
        }
    }

    class SessionContinuePacket : IPacket
    {
        public PacketType Type { get { return PacketType.SessionContinue; } }
        public string SessionID { get; set; }
        public string BuildPacket()
        {
            return $"{(int)Type}%{SessionID}";
        }
    }


}
