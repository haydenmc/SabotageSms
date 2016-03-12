using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models.DbModels
{
    [Table("Player")]
    public class DbPlayer : Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        new public long PlayerId { get; set; }
        
        [MaxLength(50)]
        new public string PhoneNumber { get; set; }
        
        [ForeignKey("CurrentGameId")]
        public virtual DbGame CurrentGame { get; set; }
        
        new public long? CurrentGameId;
    }
}