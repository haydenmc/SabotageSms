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
        
        public virtual ICollection<DbGameGoodPlayer> GoodPlayers { get; set; }
        
        public virtual ICollection<DbGameBadPlayer> BadPlayers { get; set; }

        public GameState CurrentState { get; set; }

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
                GoodPlayers = GoodPlayers?.Select(p => p.Player.ToPlayer()).ToList(),
                BadPlayers = BadPlayers?.Select(p => p.Player.ToPlayer()).ToList(),
                CurrentState = CurrentState,
                CreatedTime = CreatedTime,
                LastActiveTime = LastActiveTime
            };
        }
    }
}