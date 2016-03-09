using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models {
    public class Game {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GameId { get; set; }
        
        [MaxLength(120)]
        public string JoinCode { get; set; }
        
        [InverseProperty("CurrentGame")]
        public virtual ICollection<Player> Players { get; set; }
        
        public virtual ICollection<Player> GoodPlayers { get; set; }
        
        public virtual ICollection<Player> BadPlayers { get; set; }
        
        public DateTimeOffset CreatedTime { get; set; }
        
        public DateTimeOffset LastActiveTime { get; set; }
    }
}