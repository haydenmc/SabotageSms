using System;
using System.Linq;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl.States
{
    public class GameOverState : AbstractState
    {
        public GameOverState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
            : base(gameDataProvider, smsProvider, game)
        {}
        
        public void AnnounceWinner()
        {
             // Determine game end
            var badWinCount = _game.Rounds.Where(r => r.BadWins).Count();
            var goodWinCount = _game.Rounds.Where(r => !r.BadWins).Count();
            var badNames = string.Join(", ", _game.BadPlayers.Select(p => p.Name));
            if (badWinCount > goodWinCount)
            {
                SmsAll(string.Format(GameStrings.SaboteursWin, badNames));
            }
            else
            {
                SmsAll(string.Format(GameStrings.SaboteursLose, badNames));
            }
        }
        
        public void SaboteursWin()
        {
            var badNames = string.Join(", ", _game.BadPlayers.Select(p => p.Name));
            SmsAll(string.Format(GameStrings.SaboteursWin, badNames));
        }

        public override AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters)
        {
            return null;
        }
    }
}