using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class RosterApprovalState : AbstractState
    {
        public RosterApprovalState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}
        
        public void AnnounceStart()
        {
            var selectedPlayerNames = string.Join(", ", _game.Rounds.Last().SelectedPlayers.Select(p => p.Name));
            SmsAll(String.Format(GameStrings.PlayersSelectedForApproval, selectedPlayerNames));
        }

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            if (command == Command.ApproveRoster || command == Command.RejectRoster)
            {
                var round = _game.Rounds.Last();
                // Mark this player as approving/rejecting
                round = _gameDataProvider.SetRoundPlayerAsApproving(round.RoundId,
                    fromPlayer.PlayerId,
                    command == Command.ApproveRoster);
                _game.Rounds[_game.Rounds.Count - 1] = round;
                
                // Check if all the votes are in.
                if ((round.ApprovingPlayers.Count + round.RejectingPlayers.Count) >= _game.Players.Count)
                {
                    var playerVoteSummary = String.Format(GameStrings.ApproveRejectList,
                        string.Join(", ", round.ApprovingPlayers.Select(p => p.Name)),
                        string.Join(", ", round.RejectingPlayers.Select(p => p.Name)));
                    if (round.ApprovingPlayers.Count > round.RejectingPlayers.Count)
                    {
                        // Mission is approved.
                        SmsAll(String.Format(GameStrings.MissionApproved, playerVoteSummary));
                        // Advance to mission state.
                        var missionState = new MissionState(_gameDataProvider, _smsProvider, _game);
                        missionState.Announce();
                        return missionState;
                    }
                    else
                    {
                        // Mission is rejected.
                        var numRejections = round.RejectedCount + 1;
                        round = _gameDataProvider.SetRoundRejectedCount(round.RoundId, numRejections);
                        _game.Rounds[_game.Rounds.Count - 1] = round;
                        if (numRejections > GameManager.MaxRejectionCount)
                        {
                            // Transition to game over state
                            var gameOverState = new GameOverState(_gameDataProvider, _smsProvider, _game);
                            gameOverState.SaboteursWin();
                            return gameOverState;
                        }
                        else
                        {
                            SmsAll(String.Format(GameStrings.MissionRejected,
                                GameManager.MaxRejectionCount - numRejections,
                                playerVoteSummary));
                            // Clear approvals and advance to next leader
                            round = _gameDataProvider.ClearRoundApprovals(round.RoundId);
                            _game = _gameDataProvider.AdvanceGameLeader(_game.GameId);
                            // Advance to roster state
                            var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game);
                            rosterState.Announce();
                            return rosterState;
                        }
                    }
                }
                else // Not all votes tallied yet
                {
                    SmsPlayer(fromPlayer,
                        String.Format(GameStrings.ResponseRecordedWaiting, 
                            _game.Players.Count - (round.ApprovingPlayers.Count + round.RejectingPlayers.Count)));
                    return this;
                }
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}