using System.Collections.Generic;
using System.Linq;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl.States
{
    public abstract class AbstractState
    {
        protected AbstractState _resetState { get; set; }
        
        protected IGameDataProvider _gameDataProvider { get; set; }
        
        protected ISmsProvider _smsProvider { get; set; }
        
        protected Game _game { get; set; }
        
        public AbstractState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game, AbstractState resetState)
        {
            _gameDataProvider = gameDataProvider;
            _smsProvider = smsProvider;
            _game = game;
            _resetState = resetState;
        }
        
        public abstract AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters);
        
        protected void SmsPlayer(Player player, string body)
        {
            if (player != null)
            {
                _smsProvider.SendSms(player.PhoneNumber, body);
            }
        }
        
        protected void SmsPlayer(long playerId, string body)
        {
            var player = _game.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player != null)
            {
                SmsPlayer(player, body);
            }
        }
        
        protected void SmsAll(string body)
        {
            for (var i = 0; i < _game.Players.Count; i++)
            {
                _smsProvider.SendSms(_game.Players[i].PhoneNumber, body);
            }
        }
        
        protected void SmsAllExcept(IEnumerable<Player> players, string body)
        {
            for (var i = 0; i < _game.Players.Count; i++)
            {
                if (players.Where(p => p.PlayerId == _game.Players[i].PlayerId).Count() == 0)
                {
                    _smsProvider.SendSms(_game.Players[i].PhoneNumber, body);
                }
            }
        }
        
        protected void SmsAllExcept(Player player, string body)
        {
            SmsAllExcept(new Player[] { player }, body);
        }
        
        protected void SmsAllExcept(long playerId, string body)
        {
            var player = _game.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player != null)
            {
                SmsAllExcept(player, body);
            }
        }
    }
}