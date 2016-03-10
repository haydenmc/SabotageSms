using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models {
    public class GamePlayer {
        public long PlayerId { get; set; }
        
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }
        
        public long GameId { get; set; }
        
        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
        
        public int TurnOrder { get; set; }
    }
}