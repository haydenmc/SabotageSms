using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class RosterApprovalState : AbstractState
    {
        public RosterApprovalState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
            : base(gameDataProvider, smsProvider, game, resetState)
        {}
        
        public void AnnounceStart()
        {
            var selectedPlayerNames = string.Join(", ", _game.Rounds.Last().SelectedPlayers.Select(p => p.Name));
            SmsAll(String.Format("{0} have been selected. Vote 'approve' or 'reject'.", selectedPlayerNames));
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
                    var playerVoteSummary = String.Format("✔: {0}\n✖: {1}",
                        string.Join(", ", round.ApprovingPlayers.Select(p => p.Name)),
                        string.Join(", ", round.RejectingPlayers.Select(p => p.Name)));
                    if (round.ApprovingPlayers.Count > round.RejectingPlayers.Count)
                    {
                        // Mission is approved.
                        SmsAll(String.Format("MISSION APPROVED.\n{0}", playerVoteSummary));
                        // Advance to mission state.
                        var missionState = new MissionState(_gameDataProvider, _smsProvider, _game, _resetState);
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
                            // TODO: bad player victory
                        }
                        else
                        {
                            SmsAll(String.Format("MISSION REJECTED. {0} rejections remain.\n{1}",
                                GameManager.MaxRejectionCount - numRejections,
                                playerVoteSummary));
                            // Clear approvals and advance to next leader
                            round = _gameDataProvider.ClearRoundApprovals(round.RoundId);
                            _game = _gameDataProvider.AdvanceGameLeader(_game.GameId);
                            // Advance to roster state
                            var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game, _resetState);
                            rosterState.Announce();
                            return rosterState;
                        }
                    }
                }
                else // Not all votes tallied yet
                {
                    SmsPlayer(fromPlayer,
                        String.Format("Response recorded. Still waiting on {0} players.", 
                            _game.Players.Count - (round.ApprovingPlayers.Count + round.RejectingPlayers.Count)));
                    return this;
                }
            }
            // Unimplemented command for this state.
            SmsPlayer(fromPlayer, "⚠ You can't do that right now.");
            return this;
        }
    }
}