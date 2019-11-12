namespace Core.Models
{
    public class GameServerStatus
    {
        public GameServerStatus() { }
        public GameServerStatus(string name, int players, int maxPlayers, string map, int port)
        {
            Name = name;
            Players = players;
            MaxPlayers = maxPlayers;
            Map = map;
            Port = port;
        }

        public string Name { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public string Map { get; set; }
        public int Port { get; set; }
    }
}