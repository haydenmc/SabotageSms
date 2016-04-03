using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class NoGameState : AbstractState
    {
        public NoGameState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            // New game command
            if (command == Command.New)
            {
                _game = _gameDataProvider.CreateNewGame(fromPlayer.PlayerId);
                SmsPlayer(fromPlayer,
                    String.Format("New game created! Tell others to text 'Join {0}' to this number.", _game.JoinCode));
                return new LobbyState(_gameDataProvider, _smsProvider, _game, this);
            }
            // Join game command
            if (command == Command.Join)
            {
                var game = _gameDataProvider.GetGameByJoinCode(parameters as string);
                if (game == null)
                {
                    SmsPlayer(fromPlayer, "We couldn't find that game.");
                    return this;
                }
                if (game.CurrentState != typeof(LobbyState).Name)
                {
                    SmsPlayer(fromPlayer, "You can't join a game in progress.");
                    return this;
                }
                if (game.Players.Count >= GameManager.MaxPlayers) {
                    SmsPlayer(fromPlayer, "This game is full.");
                    return this;
                }
                _game = _gameDataProvider.JoinPlayerToGame(fromPlayer.PlayerId, game.GameId);
                SmsPlayer(fromPlayer,
                    String.Format("You have joined the game! There are {0} players currently in this game.", _game.Players.Count));
                SmsAllExcept(fromPlayer,
                    String.Format("{0} has joined the game! There are {1} players currently in this game.", fromPlayer.Name, _game.Players.Count));
                return new LobbyState(_gameDataProvider, _smsProvider, _game, this);
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}