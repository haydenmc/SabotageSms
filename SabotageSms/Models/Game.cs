using System;
using System.Collections.Generic;

namespace SabotageSms.Models
{   
    public class Game
    {
        public long GameId { get; set; }
        public string JoinCode { get; set; }
        public IList<Player> Players { get; set; }
        public int MissionCount { get; set; }
        public int LeaderCount { get; set; }
        public ICollection<Player> GoodPlayers { get; set; }
        public ICollection<Player> BadPlayers { get; set; }
        public string CurrentState { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastActiveTime { get; set; }
    }
}