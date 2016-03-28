using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SabotageSms.Models.DbModels
{
    [Table("Game")]
    public class DbGame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GameId { get; set; }
        
        [MaxLength(120)]
        public string JoinCode { get; set; }
        
        public virtual ICollection<DbGamePlayer> GamePlayers { get; set; }
        
        [InverseProperty("Game")]
        public virtual ICollection<DbRound> Rounds { get; set; }
        
        public int MissionCount { get; set; }
        
        public int LeaderCount { get; set; }

        public string CurrentState { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset LastActiveTime { get; set; }
        
        public Game ToGame() {
            return new Game()
            {
                GameId = GameId,
                JoinCode = JoinCode,
                Players = GamePlayers?
                    .OrderBy(gp => gp.TurnOrder)
                    .Select(gp => gp.Player.ToPlayer())
                    .ToList(),
                Rounds = Rounds?.OrderBy(r => r.RoundNumber).Select(r => r.ToRound()).ToList(),
                LeaderCount = LeaderCount,
                GoodPlayers = GamePlayers?.Where(p => !p.IsBad).Select(p => p.Player.ToPlayer()).ToList(),
                BadPlayers = GamePlayers?.Where(p => p.IsBad).Select(p => p.Player.ToPlayer()).ToList(),
                CurrentState = CurrentState,
                CreatedTime = CreatedTime,
                LastActiveTime = LastActiveTime
            };
        }
    }
}