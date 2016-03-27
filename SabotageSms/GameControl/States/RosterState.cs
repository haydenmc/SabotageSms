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
        
        public void AnnounceStart()
        {
            // Grab the current mission leader
            var leader = _game.Players[_game.LeaderCount % _game.Players.Count];
            
            // Announce start of roster state
            SmsAllExcept(leader,
                String.Format("{0} is the mission leader. They will now select players for this mission.", leader.Name));
            SmsPlayer(leader, "You are the mission leader. Please select players for this mission.");
        }

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}