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
            // Unimplemented command for this state.
            SmsPlayer(fromPlayer, "⚠ You can't do that right now.");
            return this;
        }
    }
}