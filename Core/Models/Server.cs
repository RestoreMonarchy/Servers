using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class GameServer
    {
        public GameServer()  { }

        public GameServer(int id, string shortName, string name, string address, string ip, int port, int queryPort)
        {
            ServerId = id;
            ShortName = shortName;
            Name = name;
            Address = address;
            IP = ip;
            Port = port;
            QueryPort = queryPort;
        }

        public int ServerId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int QueryPort { get; set; }


        public virtual GameServerStatus Status { get; set; }
    }
}
