using System;
using SabotageSms.Models;
using SabotageSms.Providers;
using System.Linq;

namespace SabotageSms.GameControl.States
{
    public class GameOverState : AbstractState
    {
        public GameOverState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
            : base(gameDataProvider, smsProvider, game, resetState)
        {}

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}