﻿using System;
using System.Collections.Generic;
using System.Text;

namespace kpssOrganizerServer_TEST
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
        Login_Fail_AccoundBanned = 204
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

    public class ResponsePacket : IPacket
    {
        public PacketType Type { get { return PacketType.Response; } }
        public ResponseCode Code { get; set; }
        public string Extra { get; set; }
        public string BuildPacket()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((int)Type + "%" + (int)Code);
            if (Extra != null) sb.Append("%" + Extra);

            return sb.ToString();
        }
    }


}