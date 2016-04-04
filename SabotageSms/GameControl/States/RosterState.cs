using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class RosterState : AbstractState
    {
        public RosterState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}
        
        public void Announce()
        {
            // Check required fail count
            var failCountWarning = "";
            var missionFailCount = GameManager.MissionRequiredFailCount[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
            if (missionFailCount != 1)
            {
                failCountWarning = String.Format("\n" + GameStrings.FailCountWarning, missionFailCount);
            }
            
            // Grab the current mission leader
            var leader = _game.Players[_game.LeaderCount % _game.Players.Count];
            
            // Announce start of roster state
            var missionPlayerCount = GameManager.MissionPlayerNumber[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
            SmsAllExcept(leader,
                String.Format(GameStrings.NewRoundAnnounce, _game.Rounds.Count, leader.Name, missionPlayerCount, failCountWarning));
            SmsPlayer(leader,
                String.Format(GameStrings.NewRoundAnnounceForLeader, _game.Rounds.Count, missionPlayerCount, failCountWarning));
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
                        String.Format(GameStrings.OnlyLeaderCanSelectRoster, leader.Name));
                    return this;
                }
                
                var players = parameters as string[];
                
                // Make sure we have the right number of players
                var missionPlayerCount = GameManager.MissionPlayerNumber[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
                if (players.Length != missionPlayerCount)
                {
                    SmsPlayer(fromPlayer,
                        String.Format(GameStrings.MustSelectNumberOfPlayers, missionPlayerCount));
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
                            String.Format(GameStrings.CouldNotFindPlayerByName, players[i]));
                        return this;
                    }
                    playerIds[i] = player.PlayerId;
                }
                
                // Commit players and announce
                var round = _gameDataProvider.SetRoundSelectedPlayers(_game.Rounds.Last().RoundId, playerIds);
                _game.Rounds[_game.Rounds.Count - 1] = round;
                var selectedPlayerNames = string.Join(", ", round.SelectedPlayers.Select(p => p.Name));
                SmsPlayer(fromPlayer, 
                    String.Format(GameStrings.RosterSelectedForLeader, selectedPlayerNames));
                SmsAllExcept(fromPlayer, 
                    String.Format(GameStrings.RosterSelected, selectedPlayerNames, fromPlayer.Name));
                return new RosterStagingState(_gameDataProvider, _smsProvider, _game);
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}