using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models {
    public class Player {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PlayerId { get; set; }
        
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        
        public string Name { get; set; }
        
        [ForeignKey("CurrentGameId")]
        public virtual Game CurrentGame { get; set; }
        
        public long CurrentGameId { get; set; }
    }
}