using System.Collections.Generic;

namespace SabotageSms.Models
{
    public class Round
    {
        public long RoundId { get; set; }
        public long GameId { get; set; }
        public int RoundNumber { get; set; }
        public int RejectedCount { get; set; }
        public ICollection<Player> SelectedPlayers { get; set; }
        public ICollection<Player> ApprovingPlayers { get; set; }
        public ICollection<Player> RejectingPlayers { get; set; }
        public ICollection<Player> PassingPlayers { get; set; }
        public ICollection<Player> FailingPlayers { get; set; }
    }
}