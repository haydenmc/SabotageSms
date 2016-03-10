using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models {
    
    public class Message {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MessageId { get; set; }
        
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }
        
        public long PlayerId { get; set; }
        
        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
        
        public long? GameId { get; set; }
        
        public string Body { get; set; }
        
        public DateTimeOffset ReceivedTime { get; set; }
        
        public MessageResult Result { get; set; }
    }
    public enum MessageResult {
        Success = 0,
        ParseError = 1,
        GameError = 2
    }
}