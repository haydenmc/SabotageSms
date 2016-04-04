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
        
        private AbstractState PlayerDepart(Player fromPlayer)
        {
            // If the game hasn't started yet, they simply leave. Otherwise, they forfeit.
            if (_game.CurrentState == typeof(LobbyState).Name)
            {
                SmsAllExcept(fromPlayer,
                    string.Format(GameStrings.PlayerLeftLobby, fromPlayer.Name, _game.Players.Count - 1));
                _game = _gameDataProvider.RemovePlayerFromGame(_game.GameId, fromPlayer.PlayerId);
                return this;
            }
            else if (_game.CurrentState == typeof(GameOverState).Name)
            {
                // Do nothing, the game is already over.
                return this;
            }
            else
            {
                SmsAll(string.Format(GameStrings.PlayerForfeits, fromPlayer.Name));
                // Transition to game over state
                var gameOverState = new GameOverState(_gameDataProvider, _smsProvider, _game);
                gameOverState.AnnounceWinner();
                return gameOverState;
            }
        }

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
                AbstractState returnState = this;
                if (_game != null)
                {
                    // The state we return will apply to our last game
                    returnState = PlayerDepart(fromPlayer);
                }
                _game = _gameDataProvider.CreateNewGame(fromPlayer.PlayerId);
                SmsPlayer(fromPlayer,
                    String.Format(GameStrings.NewGameCreated, _game.JoinCode));
                return returnState;
            }
            
            // Join game command
            if (command == Command.Join)
            {
                AbstractState returnState = this;
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
                if (_game != null)
                {
                    // The state we return will apply to our last game
                    returnState = PlayerDepart(fromPlayer);
                }
                _game = _gameDataProvider.JoinPlayerToGame(fromPlayer.PlayerId, game.GameId);
                SmsPlayer(fromPlayer,
                    String.Format(GameStrings.YouHaveJoined, _game.Players.Count));
                SmsAllExcept(fromPlayer,
                    String.Format(GameStrings.NewPlayerJoined, fromPlayer.Name, _game.Players.Count));
                return returnState;
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}