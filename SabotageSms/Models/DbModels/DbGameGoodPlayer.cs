using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models.DbModels
{
    [Table("GameGoodPlayer")]
    public class DbGameGoodPlayer
    {
        public long PlayerId { get; set; }
        
        [ForeignKey("PlayerId")]
        public virtual DbPlayer Player { get; set; }
        
        public long GameId { get; set; }
        
        [ForeignKey("GameId")]
        public virtual DbGame Game { get; set; }
    }
}