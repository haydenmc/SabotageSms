namespace SabotageSms.Models
{
    public class Player
    {
        public long PlayerId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public long? CurrentGameId { get; set; }
    }
}