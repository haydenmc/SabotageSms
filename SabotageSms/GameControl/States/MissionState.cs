using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class MissionState : AbstractState
    {
        public MissionState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}
        
        public void Announce()
        {
            var round = _game.Rounds.Last();
            SmsAllExcept(round.SelectedPlayers,
                String.Format(GameStrings.MissionStart,
                    String.Join(", ", round.SelectedPlayers.Select(p => p.Name))));
            foreach (var p in round.SelectedPlayers)
            {
                var otherPlayerNames
                    = String.Join(", ", round
                        .SelectedPlayers
                        .Where(sp => sp.PlayerId != p.PlayerId)
                        .Select(sp => sp.Name));
                SmsPlayer(p, String.Format(GameStrings.MissionStartYou, otherPlayerNames));
            }
        }

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            if (command == Command.PassMission || command == Command.FailMission)
            {
                var round = _game.Rounds.Last();
                if (round.SelectedPlayers.SingleOrDefault(sp => sp.PlayerId == fromPlayer.PlayerId) == null)
                {
                    SmsPlayer(fromPlayer, GameStrings.YouAreNotOnThisMission);
                    return this;
                }
                
                // Record their submission
                round = _gameDataProvider.SetRoundPlayerPassFail(
                    round.RoundId,
                    fromPlayer.PlayerId,
                    command == Command.PassMission);
                _game.Rounds[_game.Rounds.Count - 1] = round;
                
                // Check if all submissions are in
                if ((round.PassingPlayers.Count + round.FailingPlayers.Count) >= round.SelectedPlayers.Count)
                {
                    var missionFailCount = GameManager.MissionRequiredFailCount[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
                    if (round.FailingPlayers.Count >= missionFailCount)
                    {
                        round = _gameDataProvider.SetRoundBadWins(round.RoundId, true);
                        SmsAll(String.Format(GameStrings.MissionSabotaged, round.PassingPlayers.Count, round.FailingPlayers.Count));
                    }
                    else
                    {
                        round = _gameDataProvider.SetRoundBadWins(round.RoundId, false);
                        SmsAll(String.Format(GameStrings.MissionSucceeded, round.PassingPlayers.Count, round.FailingPlayers.Count));
                    }
                    
                    // Update round in game reference
                    _game.Rounds[_game.Rounds.Count - 1] = round;
                    
                    // Determine game end
                    var badWinCount = _game.Rounds.Where(r => r.BadWins).Count();
                    var goodWinCount = _game.Rounds.Where(r => !r.BadWins).Count();
                    if (badWinCount >= GameManager.WinCount || goodWinCount >= GameManager.WinCount)
                    {
                        // Transition to game over state
                        var gameOverState = new GameOverState(_gameDataProvider, _smsProvider, _game);
                        gameOverState.AnnounceWinner();
                        return gameOverState;
                    }
                    else
                    {
                        // Add new round
                        _game = _gameDataProvider.AddRound(_game.GameId);
                        // Advance leader
                        _game = _gameDataProvider.AdvanceGameLeader(_game.GameId);
                        // Advance to roster state
                        var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game);
                        rosterState.Announce();
                        return rosterState;
                    }
                }
                else
                {
                    SmsPlayer(fromPlayer,
                        String.Format(GameStrings.ResponseRecordedWaiting, 
                            round.SelectedPlayers.Count - (round.PassingPlayers.Count + round.FailingPlayers.Count)));
                    return this;
                }
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}