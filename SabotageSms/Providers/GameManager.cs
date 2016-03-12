using System;
using System.Collections.Generic;
using SabotageSms.Models;

namespace SabotageSms.Providers
{
    public class GameManager
    {
        private Game _game { get; set; }
        
        private ISmsProvider _smsProvider { get; set; }
        
        private readonly Dictionary<Tuple<GameState, CommandType>, CommandExecution> _stateTransitions;
        
        private delegate CommandResult CommandExecution(Player player, CommandDetails command);
        
        public GameManager(Game game, ISmsProvider smsProvider)
        {
            _game = game;
            _smsProvider = smsProvider;
            
            // State machine layout
            // TODO: Find a way to get this into a static space so we're not populating it every time...
            _stateTransitions = new Dictionary<Tuple<GameState, CommandType>, CommandExecution>()
            {
                { new Tuple<GameState, CommandType>(GameState.Any, CommandType.New), New },
                { new Tuple<GameState, CommandType>(GameState.Any, CommandType.Join), Join },
                { new Tuple<GameState, CommandType>(GameState.Lobby, CommandType.StartGame), StartGame },
                { new Tuple<GameState, CommandType>(GameState.Roster, CommandType.SelectRoster), SelectRoster },
                { new Tuple<GameState, CommandType>(GameState.RosterApproval, CommandType.RosterCountVote), RosterCountVote },
                { new Tuple<GameState, CommandType>(GameState.RosterApproval, CommandType.RejectRoster), RejectRoster },
                { new Tuple<GameState, CommandType>(GameState.RosterApproval, CommandType.ApproveRoster), ApproveRoster },
                { new Tuple<GameState, CommandType>(GameState.Mission, CommandType.MissionCountSuccessFail), MissionCountSuccessFail },
                { new Tuple<GameState, CommandType>(GameState.Mission, CommandType.NewMission), NewMission },
                { new Tuple<GameState, CommandType>(GameState.Mission, CommandType.GameEnd), GameEnd },
            };
        }
        
        public void Command(Player fromPlayer, CommandDetails details)
        {
            // Check command against current game state
            var transitionKey = new Tuple<GameState, CommandType>(_game.CurrentState, details.CommandType);
            if (!_stateTransitions.ContainsKey(transitionKey))
            {
                // Check against state-neutral commands
                transitionKey = new Tuple<GameState, CommandType>(GameState.Any, details.CommandType);
                if (!_stateTransitions.ContainsKey(transitionKey))
                {
                    _smsProvider.SendSms(fromPlayer.PhoneNumber, "You can't run this command right now.");
                    return;
                }
            }
            var transitionCommand = _stateTransitions[transitionKey];
            var result = transitionCommand(fromPlayer, details);
        }
        
        private CommandResult New(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult Join(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult StartGame(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult SelectRoster(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult RosterCountVote(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult RejectRoster(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult ApproveRoster(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult MissionCountSuccessFail(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult NewMission(Player player, CommandDetails command)
        {
            
        }
        
        private CommandResult GameEnd(Player player, CommandDetails command)
        {
            
        }
    }
}