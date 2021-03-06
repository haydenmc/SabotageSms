using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SabotageSms.Models.DbModels
{
    [Table("Message")]
    public class DbMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MessageId { get; set; }
        
        [ForeignKey("PlayerId")]
        public virtual DbPlayer Player { get; set; }
        
        public long PlayerId { get; set; }
        
        [ForeignKey("GameId")]
        public virtual DbGame Game { get; set; }
        
        public long? GameId { get; set; }
        
        public string Body { get; set; }
        
        public DateTimeOffset ReceivedTime { get; set; }
        
        public MessageResult Result { get; set; }
    }
    public enum MessageResult
    {
        Success = 0,
        ParseError = 1,
        GameError = 2
    }
}