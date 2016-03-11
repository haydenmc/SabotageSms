using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SabotageSms.Models.DbModels {
    [Table("Game")]
    public class DbGame : Game {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        new public long GameId { get; set; }
        
        [MaxLength(120)]
        new public string JoinCode { get; set; }
        
        public ICollection<DbGamePlayer> GamePlayers { get; set; }
        
        [NotMapped]
        new public IList<DbPlayer> Players {
            get {
                return GamePlayers
                           .OrderBy(gp => gp.TurnOrder)
                           .Select(gp => gp.Player)
                           .ToList();
            }
        }
        
        new public virtual ICollection<DbPlayer> GoodPlayers { get; set; }
        
        new public virtual ICollection<DbPlayer> BadPlayers { get; set; }
    }
}