using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class MissionState : AbstractState
    {
        public MissionState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
            : base(gameDataProvider, smsProvider, game, resetState)
        {}
        
        public void Announce()
        {
            var round = _game.Rounds.Last();
            SmsAllExcept(round.SelectedPlayers,
                String.Format("MISSION START: {0}.",
                    String.Join(", ", round.SelectedPlayers.Select(p => p.Name))));
            foreach (var p in round.SelectedPlayers)
            {
                var otherPlayerNames
                    = String.Join(", ", round
                        .SelectedPlayers
                        .Where(sp => sp.PlayerId != p.PlayerId)
                        .Select(sp => sp.Name));
                SmsPlayer(p, String.Format("MISSION START: You, {0}. 'Pass' or 'Fail'.", otherPlayerNames));
            }
        }

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            if (command == Command.PassMission || command == Command.FailMission)
            {
                var round = _game.Rounds.Last();
                if (round.SelectedPlayers.SingleOrDefault(sp => sp.PlayerId == fromPlayer.PlayerId) == null)
                {
                    SmsPlayer(fromPlayer, "⚠ You were not selected for this mission.");
                    return this;
                }
                
                // Record their submission
                _game = _gameDataProvider.SetRoundPlayerPassFail(
                    round.RoundId,
                    fromPlayer.PlayerId,
                    command == Command.PassMission);
                round = _game.Rounds.Last();
                
                // Check if all submissions are in
                if ((round.PassingPlayers.Count + round.FailingPlayers.Count) >= round.SelectedPlayers.Count)
                {
                    var missionFailCount = GameManager.MissionRequiredFailCount[_game.Rounds.Count - 1, _game.Players.Count - GameManager.MinPlayers];
                    if (round.FailingPlayers.Count >= missionFailCount)
                    {
                        _game = _gameDataProvider.SetRoundBadWins(round.RoundId, true);
                        SmsAll(String.Format("MISSION SABOTAGED: {0} pass, {1} fail.", round.PassingPlayers.Count, round.FailingPlayers.Count));
                    }
                    else
                    {
                        _game = _gameDataProvider.SetRoundBadWins(round.RoundId, false);
                        SmsAll(String.Format("MISSION SUCCEEDED: {0} pass, {1} fail.", round.PassingPlayers.Count, round.FailingPlayers.Count));
                    }
                    
                    // Determine game end
                    var badWinCount = _game.Rounds.Where(r => r.BadWins).Count();
                    var goodWinCount = _game.Rounds.Where(r => !r.BadWins).Count();
                    if (badWinCount >= GameManager.WinCount || goodWinCount >= GameManager.WinCount)
                    {
                        var badNames = String.Format("👿: {0}", String.Join(", ", _game.BadPlayers.Select(p => p.Name)));
                        if (badWinCount > goodWinCount)
                        {
                            SmsAll(String.Format("GAME OVER: BAD WINS.\n{0}", badNames));
                        }
                        else
                        {
                            SmsAll(String.Format("GAME OVER: GOOD WINS.\n{0}", badNames));
                        }
                        // Transition to game over state
                        return new GameOverState(_gameDataProvider, _smsProvider, _game, _resetState);
                    }
                    else
                    {
                        // Add new round
                        _game = _gameDataProvider.AddRound(_game.GameId);
                        // Advance leader
                        _game = _gameDataProvider.AdvanceGameLeader(_game.GameId);
                        // Advance to roster state
                        var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game, _resetState);
                        rosterState.Announce();
                        return rosterState;
                    }
                }
                else
                {
                    SmsPlayer(fromPlayer,
                        String.Format("Response recorded. Still waiting on {0} players.", 
                            round.SelectedPlayers.Count - (round.PassingPlayers.Count + round.FailingPlayers.Count)));
                    return this;
                }
            }
            
            // Unimplemented command for this state.
            SmsPlayer(fromPlayer, "⚠ You can't do that right now.");
            return this;
        }
    }
}