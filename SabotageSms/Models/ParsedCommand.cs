using SabotageSms.GameControl;

namespace SabotageSms.Models
{
    public class ParsedCommand
    {
        public Command Command { get; set; }
        public object Parameters { get; set; }
    }
}