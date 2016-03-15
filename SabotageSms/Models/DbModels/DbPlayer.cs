using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models.DbModels
{
    [Table("Player")]
    public class DbPlayer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PlayerId { get; set; }
        
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        
        public string Name { get; set;}
        
        [ForeignKey("CurrentGameId")]
        public virtual DbGame CurrentGame { get; set; }
        
        public long? CurrentGameId { get; set; }
        
        public Player ToPlayer() {
            return new Player()
            {
                PlayerId = PlayerId,
                PhoneNumber = PhoneNumber,
                Name = Name,
                CurrentGameId = CurrentGameId
            };
        }
    }
}