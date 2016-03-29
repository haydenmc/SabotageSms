using System;
using System.Text.RegularExpressions;
using SabotageSms.GameControl;
using SabotageSms.Models;

namespace SabotageSms.Providers
{
    public class ParsingProvider
    {
        public ParsedCommand ParseCommand(Player player, string messageBody)
        {
            var name = new Regex("(?i)name (?<name>.+)").Match(messageBody);
            if (name.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.Name,
                    Parameters = name.Groups["name"].Value
                };
            }
            
            return new ParsedCommand()
            {
                Command = Command.Unknown,
                Parameters = null
            };
        }
    }
}