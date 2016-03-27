using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class LobbyState : AbstractState
    {
        public LobbyState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
            : base(gameDataProvider, smsProvider, game, resetState)
        {}
        
        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            // Start game command
            if (command == Command.StartGame) {
                // Check for valid start conditions
                if (_game.Players.Count >= GameManager.MinPlayers
                    && _game.Players.Count <= GameManager.MaxPlayers)
                {
                    // Assign roles
                    var numBadPlayers = (int)Math.Round(Math.Sqrt(2 * (_game.Players.Count - 3)));
                    var randomPlayers = _game.Players.OrderBy(x => Guid.NewGuid());
                    var badPlayers = randomPlayers.Take(numBadPlayers);
                    var goodPlayers = randomPlayers.Skip(numBadPlayers);
                    _game = _gameDataProvider.SetPlayersGoodBad(_game.GameId, true, badPlayers.Select(p => p.PlayerId).ToArray());
                    _game = _gameDataProvider.SetPlayersGoodBad(_game.GameId, false, goodPlayers.Select(p => p.PlayerId).ToArray());
                    
                    // Inform each player of their role
                    foreach (var player in badPlayers)
                    {
                        SmsPlayer(player, "👿 You are a bad player!");
                    }
                    foreach (var player in goodPlayers)
                    {
                        SmsPlayer(player, "😎 You are a good player!");
                    }
                    
                    // Scramble turn order
                    _gameDataProvider.ScrambleTurnOrder(_game.GameId);
                    
                    // Advance to roster state
                    var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game, _resetState);
                    rosterState.AnnounceStart();
                    return rosterState;
                }
                else
                {
                    SmsPlayer(fromPlayer,
                        String.Format("You cannot start the game yet. You need between {0} and {1} players.",
                            GameManager.MinPlayers,
                            GameManager.MaxPlayers));
                }
            }
            return this;
        }
    }
}