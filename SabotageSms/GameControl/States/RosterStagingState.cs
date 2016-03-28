using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class RosterStagingState : AbstractState
    {
        public RosterStagingState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
            : base(gameDataProvider, smsProvider, game, resetState)
        {}

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            if (command == Command.SelectRoster)
            {
                var rosterState = new RosterState(_gameDataProvider, _smsProvider, _game, _resetState);
                var rosterReturn = rosterState.ProcessCommand(fromPlayer, Command.SelectRoster, parameters);
                if (rosterReturn.GetType() == typeof(RosterState))
                {
                    return this;
                }
                return rosterReturn;
            }
            if (command == Command.ConfirmRoster)
            {
                var approvalState = new RosterApprovalState(_gameDataProvider, _smsProvider, _game, _resetState);
                approvalState.AnnounceStart();
                return approvalState;
            }
            
            // Unimplemented command for this state.
            SmsPlayer(fromPlayer, "⚠ You can't do that right now.");
            return this;
        }
    }
}