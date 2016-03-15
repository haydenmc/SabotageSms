using System;
using System.Collections.Generic;
using System.Linq;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl
{
    public class GameManager
    {
        private Game _game { get; set; }
        
        private ISmsProvider _smsProvider { get; set; }
        
        private IGameDataProvider _gameDataProvider { get; set; }
        
        private readonly Dictionary<Tuple<GameState, CommandType>, CommandExecution> _stateTransitions;
        
        private delegate CommandResult CommandExecution(Player player, CommandDetails command);
        
        public GameManager(Game game, IGameDataProvider gameDataProvider, ISmsProvider smsProvider)
        {
            _game = game;
            _gameDataProvider = gameDataProvider;
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
        
        public CommandResult Command(Player fromPlayer, CommandDetails details)
        {
            Tuple<GameState, CommandType> transitionKey;
            if (_game == null) {
                transitionKey = new Tuple<GameState, CommandType>(GameState.Any, details.CommandType);
            }
            else
            {
                transitionKey = new Tuple<GameState, CommandType>(_game.CurrentState, details.CommandType);
            }
            if (!_stateTransitions.ContainsKey(transitionKey))
            {
                // Check against state-neutral commands
                transitionKey = new Tuple<GameState, CommandType>(GameState.Any, details.CommandType);
                if (!_stateTransitions.ContainsKey(transitionKey))
                {
                    _smsProvider.SendSms(fromPlayer.PhoneNumber, "You can't run this command right now.");
                    return new CommandResult()
                    {
                        IsSuccess = false,
                        ErrorMessage = "You can't run this command right now."
                    };
                }
            }
            var transitionCommand = _stateTransitions[transitionKey];
            var result = transitionCommand(fromPlayer, details);
            return result;
        }
        
        public void SmsPlayer(long playerId, string body)
        {
            var player = _game.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player != null)
            {
                _smsProvider.SendSms(player.PhoneNumber, body);
            }
        }
        
        public void SmsAll(string body)
        {
            for (var i = 0; i < _game.Players.Count; i++)
            {
                _smsProvider.SendSms(_game.Players[i].PhoneNumber, body);
            }
        }
        
        public void SmsAllExcept(long playerId, string body)
        {
            for (var i = 0; i < _game.Players.Count; i++)
            {
                if (_game.Players[i].PlayerId != playerId)
                {
                    _smsProvider.SendSms(_game.Players[i].PhoneNumber, body);
                }
            }
        }
        
        private CommandResult New(Player player, CommandDetails command)
        {
            var result = new CommandResult();
            try
            {
                _game = _gameDataProvider.CreateNewGame(player.PlayerId);
                result.IsSuccess = true;
                var smsBody = String.Format("✔ New game created! Tell others to text 'Join {0}' to this number.", _game.JoinCode);
                SmsPlayer(player.PlayerId, smsBody);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ErrorMessage = e.Message;
            }
            return result;
        }
        
        private CommandResult Join(Player player, CommandDetails command)
        {
            var result = new CommandResult();
            try 
            {
                var game = _gameDataProvider.GetGameByJoinCode(command.GameJoinCode);
                if (game == null)
                {
                    SmsPlayer(player.PlayerId, "⚠ We couldn't find that game.");
                    result.IsSuccess = false;
                    result.ErrorMessage = "A game with that join code could not be found.";
                    return result;
                }
                if (game.CurrentState != GameState.Lobby)
                {
                    SmsPlayer(player.PlayerId, "⚠ You can't join a game in progress.");
                    result.IsSuccess = false;
                    result.ErrorMessage = "This game is not currently accepting new players.";
                    return result;
                }
                _game = _gameDataProvider.JoinPlayerToGame(player.PlayerId, game.GameId);
                result.IsSuccess = true;
                var joinSuccessBody = String.Format("✔ You have joined the game! There are {0} players currently in this game.", _game.Players.Count);
                SmsPlayer(player.PlayerId, joinSuccessBody);
                var joinNotificationBody = String.Format("🙂 {0} has joined the game! There are {1} players currently in this game.", player.Name, _game.Players.Count);
                SmsAllExcept(player.PlayerId, joinNotificationBody);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ErrorMessage = e.Message;
            }
            return result;
        }
        
        private CommandResult StartGame(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult SelectRoster(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult RosterCountVote(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult RejectRoster(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult ApproveRoster(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult MissionCountSuccessFail(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult NewMission(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
        
        private CommandResult GameEnd(Player player, CommandDetails command)
        {
            throw new NotImplementedException();
        }
    }
}