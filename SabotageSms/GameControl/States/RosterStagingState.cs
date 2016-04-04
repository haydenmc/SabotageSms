using System;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl.States
{
    public class RosterStagingState : AbstractState
    {
        public RosterStagingState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            if (command == Command.SelectRoster)
            {
                var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game);
                var rosterReturn = rosterState.ProcessCommand(fromPlayer, Command.SelectRoster, parameters);
                if (rosterReturn.GetType() == typeof(RosterState))
                {
                    return this;
                }
                return rosterReturn;
            }
            if (command == Command.ConfirmRoster)
            {
                // Make sure the right player is sending this command
                var leader = _game.Players[_game.LeaderCount % _game.Players.Count];
                if (fromPlayer.PlayerId != leader.PlayerId)
                {
                    SmsPlayer(fromPlayer,
                        String.Format(GameStrings.OnlyLeaderCanConfirmRoster, leader.Name));
                    return this;
                }
                var approvalState = new RosterApprovalState(_gameDataProvider, _smsProvider, _game);
                approvalState.AnnounceStart();
                return approvalState;
            }
            
            // Unimplemented command for this state.
            return null;
        }
    }
}