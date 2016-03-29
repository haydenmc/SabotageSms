using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SabotageSms.Models.DbModels
{
    [Table("Round")]
    public class DbRound
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RoundId { get; set; }
        
        public long GameId { get; set; }
        
        [ForeignKey("GameId")]
        [InverseProperty("Rounds")]
        public virtual DbGame Game { get; set; }
        
        public int RoundNumber { get; set; }
        
        public int RejectedCount { get; set; }
        
        public bool BadWins { get; set; }
        
        public virtual ICollection<DbPlayer> SelectedPlayers { get; set; }
        
        public virtual ICollection<DbPlayer> ApprovingPlayers { get; set; }
        
        public virtual ICollection<DbPlayer> RejectingPlayers { get; set; }
        
        public virtual ICollection<DbPlayer> PassingPlayers { get; set; }
        
        public virtual ICollection<DbPlayer> FailingPlayers { get; set; }
        
        public Round ToRound()
        {
            return new Round()
            {
                RoundId = RoundId,
                GameId = GameId,
                RoundNumber = RoundNumber,
                RejectedCount = RejectedCount,
                BadWins = BadWins,
                SelectedPlayers = SelectedPlayers.Select(p => p.ToPlayer()).ToList(),
                ApprovingPlayers = ApprovingPlayers.Select(p => p.ToPlayer()).ToList(),
                RejectingPlayers = RejectingPlayers.Select(p => p.ToPlayer()).ToList(),
                PassingPlayers = PassingPlayers.Select(p => p.ToPlayer()).ToList(),
                FailingPlayers = FailingPlayers.Select(p => p.ToPlayer()).ToList()
            };
        }
    }
}