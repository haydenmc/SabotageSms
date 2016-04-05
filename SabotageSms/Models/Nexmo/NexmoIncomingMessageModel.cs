namespace SabotageSms.Models
{
    public class NexmoIncomingMessageModel
    {
        public string To { get; set; }
        public string Msisdn { get; set; }
        public string Text { get; set; }
    }
}