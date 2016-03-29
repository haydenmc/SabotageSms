namespace SabotageSms.Models
{
    public class PlivoIncomingMessageModel
    {
            public string From { get; set; }
            public string To { get; set; }
            public string Type { get; set; }
            public string Text { get; set; }
            public string MessageUUID { get; set; }
    }
}