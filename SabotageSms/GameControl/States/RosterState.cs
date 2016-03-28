using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class RosterState : AbstractState
    {
        public RosterState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
            : base(gameDataProvider, smsProvider, game, resetState)
        {}
        
        public void NewRound()
        {
            // Grab the current mission leader
            var leader = _game.Players[_game.LeaderCount % _game.Players.Count];
            
            // Add new round
            _game = _gameDataProvider.AddRound(_game.GameId);
            
            // Announce start of roster state
            var missionPlayerCount = GameManager.MissionPlayerNumber[_game.Rounds.Count - 1, _game.Players.Count - 1];
            SmsAllExcept(leader,
                String.Format("ROUND {0}: {1} is the mission leader. They will now select {2} players for this mission.", _game.Rounds.Count, leader.Name, missionPlayerCount));
            SmsPlayer(leader,
                String.Format("ROUND {0}: You are the mission leader. Please select {1} players for this mission.", _game.Rounds.Count, missionPlayerCount));
        }

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            if (command == Command.SelectRoster)
            {
                // Make sure the right player is sending this command
                var leader = _game.Players[_game.LeaderCount % _game.Players.Count];
                if (fromPlayer.PlayerId != leader.PlayerId)
                {
                    SmsPlayer(fromPlayer,
                        String.Format("⚠ Only {0} can choose players to participate in this mission.", leader.Name));
                    return this;
                }
                
                var players = parameters as string[];
                
                // Make sure we have the right number of players
                var missionPlayerCount = GameManager.MissionPlayerNumber[_game.Rounds.Count - 1, _game.Players.Count - 1];
                if (players.Length != missionPlayerCount)
                {
                    SmsPlayer(fromPlayer,
                        String.Format("⚠ You need to select {0} players for this mission.", missionPlayerCount));
                    return this;
                }
                
                // Make sure players are valid in this game, get their IDs
                long[] playerIds = new long[players.Length];
                for (var i = 0; i < players.Length; i++)
                {
                    var p = _gameDataProvider.GetGamePlayerByName(_game.GameId, players[i]);
                    if (p == null)
                    {
                        SmsPlayer(fromPlayer,
                            String.Format("⚠ We couldn't find a player named '{0}'.", players[i]));
                        return this;
                    }
                    playerIds[i] = p.PlayerId;
                }
                
                // Commit players and announce
                _game = _gameDataProvider.SetRoundSelectedPlayers(_game.Rounds.Last().RoundId, playerIds);
                var selectedPlayerNames = string.Join(", ", _game.Rounds.Last().SelectedPlayers.Select(p => p.Name));
                SmsPlayer(fromPlayer, 
                    String.Format("{0} have been selected, pending your 'confirm'.", selectedPlayerNames));
                SmsAllExcept(fromPlayer, 
                    String.Format("{0} have been selected, pending {1}'s confirmation.", selectedPlayerNames, fromPlayer.Name));
                return new RosterStagingState(_gameDataProvider, _smsProvider, _game, _resetState);
            }
            
            // Unimplemented command for this state.
            SmsPlayer(fromPlayer, "⚠ You can't do that right now.");
            return this;
        }
    }
}