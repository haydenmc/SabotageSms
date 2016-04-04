using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;
using System.Text.RegularExpressions;

namespace SabotageSms.GameControl.States
{
    public class NoGameState : AbstractState
    {
        public NoGameState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            // Name command
            if (command == Command.Name)
            {
                var requestedName = parameters as string;
                var assignedName = new Regex("[^a-zA-Z]").Replace(requestedName, "");
                if (assignedName.Length > GameManager.MaxNameLength) {
                    assignedName = assignedName.Substring(0, GameManager.MaxNameLength);
                }
                if (assignedName.Length <= 0)
                {
                    SmsPlayer(fromPlayer, GameStrings.NameRequirements);
                    return this;
                }
                if (_game != null)
                {
                    // Check for duplicate names
                    if (_game.Players.Where(p => p.Name.ToUpper() == assignedName.ToUpper()).Count() > 0)
                    {
                        SmsPlayer(fromPlayer, GameStrings.DuplicateName);
                        return this;
                    }
                    SmsAllExcept(fromPlayer,
                        String.Format(GameStrings.PlayerHasChangedName, fromPlayer.Name, assignedName));
                }
                SmsPlayer(fromPlayer,
                    String.Format(GameStrings.NameSet, assignedName));
                fromPlayer = _gameDataProvider.SetPlayerName(fromPlayer.PlayerId, assignedName);
                return this;
            }
            
            // New game command
            if (command == Command.New)
            {
                _game = _gameDataProvider.CreateNewGame(fromPlayer.PlayerId);
                SmsPlayer(fromPlayer,
                    String.Format(GameStrings.NewGameCreated, _game.JoinCode));
                return new LobbyState(_gameDataProvider, _smsProvider, _game);
            }
            
            // Join game command
            if (command == Command.Join)
            {
                var game = _gameDataProvider.GetGameByJoinCode(parameters as string);
                if (game == null)
                {
                    SmsPlayer(fromPlayer, GameStrings.CouldNotFindGame);
                    return this;
                }
                if (game.CurrentState != typeof(LobbyState).Name)
                {
                    SmsPlayer(fromPlayer, GameStrings.CannotJoinGameInProgress);
                    return this;
                }
                if (game.Players.Count >= GameManager.MaxPlayers) {
                    SmsPlayer(fromPlayer, GameStrings.GameIsFull);
                    return this;
                }
                _game = _gameDataProvider.JoinPlayerToGame(fromPlayer.PlayerId, game.GameId);
                SmsPlayer(fromPlayer,
                    String.Format(GameStrings.YouHaveJoined, _game.Players.Count));
                SmsAllExcept(fromPlayer,
                    String.Format(GameStrings.NewPlayerJoined, fromPlayer.Name, _game.Players.Count));
                return new LobbyState(_gameDataProvider, _smsProvider, _game);
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}