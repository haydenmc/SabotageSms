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
        
        public void Announce()
        {
            // Check required fail count
            var failCountWarning = "";
            var missionFailCount = GameManager.MissionRequiredFailCount[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
            if (missionFailCount != 1)
            {
                failCountWarning = String.Format("\n*{0} fails required this mission.", missionFailCount);
            }
            
            // Grab the current mission leader
            var leader = _game.Players[_game.LeaderCount % _game.Players.Count];
            
            // Announce start of roster state
            var missionPlayerCount = GameManager.MissionPlayerNumber[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
            SmsAllExcept(leader,
                String.Format("ROUND {0}: {1} leader. {2} players required.{3}", _game.Rounds.Count, leader.Name, missionPlayerCount, failCountWarning));
            SmsPlayer(leader,
                String.Format("ROUND {0}: You are the leader. Select {1} players.{2}", _game.Rounds.Count, missionPlayerCount, failCountWarning));
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
                var missionPlayerCount = GameManager.MissionPlayerNumber[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
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
                    var player = _game.Players.SingleOrDefault(p => p.Name.ToUpper() == players[i].ToUpper());
                    if (player == null)
                    {
                        SmsPlayer(fromPlayer,
                            String.Format("⚠ We couldn't find a player named '{0}'.", players[i]));
                        return this;
                    }
                    playerIds[i] = player.PlayerId;
                }
                
                // Commit players and announce
                var round = _gameDataProvider.SetRoundSelectedPlayers(_game.Rounds.Last().RoundId, playerIds);
                _game.Rounds[_game.Rounds.Count - 1] = round;
                var selectedPlayerNames = string.Join(", ", round.SelectedPlayers.Select(p => p.Name));
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