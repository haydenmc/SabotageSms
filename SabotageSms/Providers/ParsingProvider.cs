using System;
using System.Text.RegularExpressions;
using SabotageSms.GameControl;
using SabotageSms.Models;

namespace SabotageSms.Providers
{
    /// <summary>
    /// Used to parse text messages into commands
    /// </summary>
    public class ParsingProvider
    {
        /// <summary>
        /// Parse a text command from a particular player into a command and its parameters
        /// </summary>
        /// <param name="player">The player who is sending the message</param>
        /// <param name="messageBody">The text content of the message</param>
        /// <returns>A parsed command with optional parameters</returns>
        public ParsedCommand ParseCommand(Player player, string messageBody)
        {
            var namePattern = new Regex("(?i)^name (?<name>.+)").Match(messageBody);
            if (namePattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.Name,
                    Parameters = namePattern.Groups["name"].Value
                };
            }
            
            var newPattern = new Regex("(?i)^new").Match(messageBody);
            if (newPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.New,
                    Parameters = null
                };
            } 
            
            var joinPattern = new Regex("(?i)^join\\s+(?<code>[a-zA-Z0-9]+)").Match(messageBody);
            if (joinPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.Join,
                    Parameters = joinPattern.Groups["code"].Value
                };
            }
            
            var startPattern = new Regex("(?i)^start").Match(messageBody);
            if (startPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.StartGame,
                    Parameters = null
                };
            }
            
            var selectRosterPattern = new Regex("(?i)^select\\s+((?<name>[a-zA-Z0-9]+)\\s*,?\\s*)+").Match(messageBody);
            if (selectRosterPattern.Success)
            {
                var names = selectRosterPattern.Groups["name"].Captures;
                var playerNames = new string[names.Count];
                for (var i = 0; i < names.Count; i++)
                {
                    playerNames[i] = names[i].Value;
                }
                return new ParsedCommand()
                {
                    Command = Command.SelectRoster,
                    Parameters = playerNames
                };
            }
            
            var confirmPattern = new Regex("(?i)^confirm").Match(messageBody);
            if (confirmPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.ConfirmRoster,
                    Parameters = null
                };
            }
            
            var rejectPattern = new Regex("(?i)^reject").Match(messageBody);
            if (rejectPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.RejectRoster,
                    Parameters = null
                };
            }
            
            var approvePattern = new Regex("(?i)^approve").Match(messageBody);
            if (approvePattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.ApproveRoster,
                    Parameters = null
                };
            }
            
            var passPattern = new Regex("(?i)^pass").Match(messageBody);
            if (passPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.PassMission,
                    Parameters = null
                };
            }
            
            var failPattern = new Regex("(?i)^fail").Match(messageBody);
            if (failPattern.Success)
            {
                return new ParsedCommand()
                {
                    Command = Command.FailMission,
                    Parameters = null
                };
            }
            
            // No recognized command.
            return new ParsedCommand()
            {
                Command = Command.Unknown,
                Parameters = null
            };
        }
    }
}