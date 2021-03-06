﻿using System;
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
        SessionContinue,
        GroupCreate,
        GroupJoin,
        GetGroupsList,
        GetGroupInfo,
        BoldDate,
        DeleteBoldedDate
    }
    public enum ResponseCode
    {
        Default = 0,
        GroupInfo = 1,
        ServerUnavailable = 2,

        Register_Success = 100,
        Register_Fail_Unknown = 101,
        Register_Fail_UsernameExists = 102,
        Register_Fail_EmailExists = 103,

        Login_Success = 200,
        Login_Fail_Unknown = 201,
        Login_Fail_IncorrectData = 202,
        Login_Fail_SessionAlreadyExists = 203,
        Login_Fail_AccoundBanned = 204,

        GroupCreate_Success = 400,
        GroupCreate_Fail_Unknown = 401,
        GroupCreate_Fail_LoginExists = 402,

        GroupJoin_Success = 500,
        GroupJoin_Fail_Unknown = 501,
        GroupJoin_Fail_IncorrectData = 502,
        GroupJoin_Fail_AccountBanned = 503,
        GroupJoin_Fail_AlreadyJoined = 504
    }

    public enum Port
    {
        Server_LoginRegister = 8880,
        Server_SessionCheck = 8881,
        Server_MainReceiver = 8882,
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

    class GroupCreatePacket : IPacket
    {
        public PacketType Type { get { return PacketType.GroupCreate; } }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SessionID { get; set; }
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
            return $"{(int)Type}%{Login}%{Password}%{SessionID}%{internalIP}";
        }
    }

    class GroupJoinPacket:IPacket
    {
        public PacketType Type { get { return PacketType.GroupJoin; } }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SessionID { get; set; }
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
            return $"{(int)Type}%{Login}%{Password}%{SessionID}%{internalIP}";
        }
    }

    class GetGroupsListPacket:IPacket
    {
        public PacketType Type { get { return PacketType.GetGroupsList; } }
        public string SessionID { get; set; }
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
            return $"{(int)Type}%{SessionID}%{internalIP}";
        }
    }

    class GetGroupInfoPacket : IPacket
    {
        public PacketType Type { get { return PacketType.GetGroupInfo; } }
        public string SessionID { get; set; }
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

        public string GroupName { get; set; }
        public string BuildPacket()
        {
            return $"{(int)Type}%{GroupName}%{SessionID}%{internalIP}";
        }
    }

    class BoldDatePacket : IPacket
    {
        public PacketType Type { get { return PacketType.BoldDate; } }
        public string GroupName { get; set; }
        public string Login { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string BuildPacket()
        {
            return $"{(int)Type}%{GroupName}%{Login}%{Date}%{Description}";
        }
    }

    class DeleteBoldedDatePacket : IPacket
    {
        public PacketType Type { get { return PacketType.DeleteBoldedDate; } }
        public string GroupName { get; set; }
        public string Date { get; set; }
        public string Login { get; set; }

        public string BuildPacket()
        {
            return $"{(int)Type}%{GroupName}%{Date}%{Login}";
        }
    }
}
