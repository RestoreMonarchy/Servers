using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Web.Client.Utilities
{
    public class SteamGameServer
    {
        #region INSTANCE VARIABLES
        private readonly IPEndPoint _endpoint;
        private readonly int _timeout;
        #endregion
        #region REQUESTS
        private static readonly byte[] PLAYER_CHALLENGE_REQUEST = new byte[] {
                0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF
            };
        private static readonly byte[] INFO_REQUEST = new byte[] {
                0xFF, 0xFF, 0xFF, 0xFF,
                0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79,
                0x00
            };
        #endregion
        #region HELPERS
        /// <summary>Reads a null-terminated string into a .NET Framework compatible string.</summary>
        /// <param name="input">Binary reader to pull the null-terminated string from.  Make sure it is correctly positioned in the stream before calling.</param>
        /// <returns>String of the same encoding as the input BinaryReader.</returns>
        public static string ReadNullTerminatedString(BinaryReader input)
        {
            var sb = new StringBuilder();
            var read = input.ReadChar();
            while (read != '\x00')
            {
                sb.Append(read);
                read = input.ReadChar();
            }
            return sb.ToString();
        }
        #endregion
        public SteamGameServer(IPEndPoint endpoint, int timeout = 5000)
        {
            _endpoint = endpoint;
            _timeout = timeout;
        }
        public async Task<Info> QueryInfoAsync() => await QueryInfoAsync(_endpoint, _timeout);
        public async Task<GameServerPlayer[]> QueryPlayersAsync() => await QueryPlayersAsync(_endpoint, _timeout);
        public static async Task<Info> QueryInfoAsync(IPEndPoint ep, int timeout = 5000)
        {
            using (var udp = new UdpClient())
            {
                udp.Client.SendTimeout = timeout;
                udp.Client.ReceiveTimeout = timeout;

                await udp.SendAsync(INFO_REQUEST, INFO_REQUEST.Length, ep);
                var result = udp.Receive(ref ep);
                using (var ms = new MemoryStream(result))
                {
                    using (var br = new BinaryReader(ms, Encoding.UTF8))
                    {
                        var info = new Info
                        {
                            Header = br.ReadByte(),
                            Protocol = br.ReadByte(),
                            Name = ReadNullTerminatedString(br),
                            Map = ReadNullTerminatedString(br),
                            Folder = ReadNullTerminatedString(br),
                            Game = ReadNullTerminatedString(br),
                            ID = br.ReadInt16(),
                            Players = br.ReadByte(),
                            MaxPlayers = br.ReadByte(),
                            Bots = br.ReadByte(),
                            ServerType = (Info.ServerTypeFlags)br.ReadByte(),
                            Environment = (Info.EnvironmentFlags)br.ReadByte(),
                            Visibility = (Info.VisibilityFlags)br.ReadByte(),
                            VAC = (Info.VACFlags)br.ReadByte(),
                            Version = ReadNullTerminatedString(br),
                            ExtraDataFlag = (Info.ExtraDataFlags)br.ReadByte()
                        };
                        if (info.ExtraDataFlag.HasFlag(Info.ExtraDataFlags.Port))
                            info.Port = br.ReadInt16();
                        if (info.ExtraDataFlag.HasFlag(Info.ExtraDataFlags.SteamID))
                            info.SteamID = br.ReadUInt64();
                        if (info.ExtraDataFlag.HasFlag(Info.ExtraDataFlags.Spectator))
                        {
                            info.SpectatorPort = br.ReadInt16();
                            info.Spectator = ReadNullTerminatedString(br);
                        }
                        if (info.ExtraDataFlag.HasFlag(Info.ExtraDataFlags.Keywords))
                            info.Keywords = ReadNullTerminatedString(br);
                        if (info.ExtraDataFlag.HasFlag(Info.ExtraDataFlags.GameID))
                            info.GameID = br.ReadUInt64();

                        return info;
                    }
                }
            }
        }
        public static async Task<GameServerPlayer[]> QueryPlayersAsync(IPEndPoint ep, int timeout = 5000)
        {
            using (var udp = new UdpClient())
            {
                udp.Client.SendTimeout = timeout;
                udp.Client.ReceiveTimeout = timeout;

                var playerReq = await GetPlayerChallengeResponseAsync(ep, udp);
                return await GetPlayersAsync(ep, udp, playerReq);
            }
        }
        private static async Task<byte[]> GetPlayerChallengeResponseAsync(IPEndPoint ep, UdpClient udp)
        {
            await udp.SendAsync(PLAYER_CHALLENGE_REQUEST, PLAYER_CHALLENGE_REQUEST.Length, ep);
            var result = udp.Receive(ref ep);
            using (var ms = new MemoryStream(result))
            {
                using (var br = new BinaryReader(ms, Encoding.UTF8))
                {
                    ms.Seek(4, SeekOrigin.Begin);   // skip the 4 0xFFs
                    var header = br.ReadByte();
                    var challengeNumber = br.ReadBytes(4);
                    return new byte[9] { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, challengeNumber[0], challengeNumber[1], challengeNumber[2], challengeNumber[3] };
                }
            }
        }
        private static async Task<GameServerPlayer[]> GetPlayersAsync(IPEndPoint ep, UdpClient udp, byte[] playerReq)
        {
            await udp.SendAsync(playerReq, playerReq.Length, ep);
            var result = udp.Receive(ref ep);
            using (var ms = new MemoryStream(result))
            {
                using (var br = new BinaryReader(ms, Encoding.UTF8))
                {
                    ms.Seek(4, SeekOrigin.Begin);   // skip the 4 0xFFs

                    var headerResp = br.ReadByte();
                    var playerCount = br.ReadByte();
                    var players = new GameServerPlayer[playerCount];

                    for (var i = 0; i < playerCount; i++)
                    {
                        var idx = br.ReadByte();
                        var name = ReadNullTerminatedString(br);
                        var score = br.ReadInt32();
                        var duration = br.ReadSingle();
                        players[i] = new GameServerPlayer()
                        {
                            Duration = duration,
                            Name = name,
                            Score = score
                        };
                    }
                    return players;
                }
            }
        }

        
    }

    public class Info
    {
        #region Enums
        [Flags]
        public enum ExtraDataFlags : byte
        {
            GameID = 0x01,
            SteamID = 0x10,
            Keywords = 0x20,
            Spectator = 0x40,
            Port = 0x80
        }
        public enum VACFlags : byte
        {
            Unsecured = 0,
            Secured = 1
        }
        public enum VisibilityFlags : byte
        {
            Public = 0,
            Private = 1
        }
        public enum EnvironmentFlags : byte
        {
            Linux = 0x6C,   //l
            Windows = 0x77, //w
            Mac = 0x6D,     //m
            MacOsX = 0x6F   //o
        }
        public enum ServerTypeFlags : byte
        {
            Dedicated = 0x64,       //d
            Nondedicated = 0x6C,    //l
            SourceTV = 0x70         //p
        }
        #endregion
        public byte Header { get; set; }
        public byte Protocol { get; set; }
        public string Name { get; set; }
        public string Map { get; set; }
        public string Folder { get; set; }
        public string Game { get; set; }
        public short ID { get; set; }
        public byte Players { get; set; }
        public byte MaxPlayers { get; set; }
        public byte Bots { get; set; }
        public ServerTypeFlags ServerType { get; set; }
        public EnvironmentFlags Environment { get; set; }
        public VisibilityFlags Visibility { get; set; }
        public VACFlags VAC { get; set; }
        public string Version { get; set; }
        public ExtraDataFlags ExtraDataFlag { get; set; }
        public ulong GameID { get; set; }           //0x01
        public ulong SteamID { get; set; }          //0x10
        public string Keywords { get; set; }        //0x20
        public string Spectator { get; set; }       //0x40
        public short SpectatorPort { get; set; }    //0x40
        public short Port { get; set; }             //0x80
    }

    public class GameServerPlayer
    {
        public virtual float Duration { get; set; }
        public virtual string Name { get; set; }
        public virtual int Score { get; set; }
    }
}
