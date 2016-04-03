using System.Collections.Generic;
using System.Linq;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl.States
{
    /// <summary>
    /// Abstract definition of a game state.
    /// A game state is responsible for taking action given certain player input.
    /// </summary>
    public abstract class AbstractState
    {
        /// <summary>
        /// Game data provider reference, used to fetch and manipulate game data.
        /// </summary>
        protected IGameDataProvider _gameDataProvider { get; set; }
        
        /// <summary>
        /// SMS provider reference. Used to send messages to game players.
        /// </summary>
        protected ISmsProvider _smsProvider { get; set; }
        
        /// <summary>
        /// Reference to game object. Manipulation of the game should only be done through
        /// the game data provider.
        /// </summary>
        protected Game _game { get; set; }
        
        public AbstractState(IGameDataProvider gameDataProvider, ISmsProvider smsProvider, Game game)
        {
            _gameDataProvider = gameDataProvider;
            _smsProvider = smsProvider;
            _game = game;
        }
        
        /// <summary>
        /// Used to advance game state given input from a player.
        /// </summary>
        /// <param name="fromPlayer">The player sending the command</param>
        /// <param name="command">The command</param>
        /// <param name="parameters">Any parameters included with the command (cast to expected type)</param>
        /// <returns>A new state to advance the game to, or null if the command wasn't understood</returns>
        public abstract AbstractState ProcessCommand(Player fromPlayer, Command command, object parameters);
        
        /// <summary>
        /// Send an SMS to a particular player
        /// </summary>
        /// <param name="player">The player to send to</param>
        /// <param name="body">The message body</param>
        protected void SmsPlayer(Player player, string body)
        {
            if (player != null)
            {
                _smsProvider.SendSms(player.PhoneNumber, body);
            }
        }
        
        /// <summary>
        /// Send an SMS to a particular player by ID
        /// </summary>
        /// <param name="playerId">The player ID to send to</param>
        /// <param name="body">The message body</param>
        protected void SmsPlayer(long playerId, string body)
        {
            var player = _game.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player != null)
            {
                SmsPlayer(player, body);
            }
        }
        
        /// <summary>
        /// Send an SMS to all players in the current game
        /// </summary>
        /// <param name="body">The message body</param>
        protected void SmsAll(string body)
        {
            for (var i = 0; i < _game.Players.Count; i++)
            {
                _smsProvider.SendSms(_game.Players[i].PhoneNumber, body);
            }
        }
        
        /// <summary>
        /// Send an SMS to all players in the current game except for the specified
        /// </summary>
        /// <param name="players">Players to exclude</param>
        /// <param name="body">The message body</param>
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
        
        /// <summary>
        /// Send an SMS to all players in the current game except for the specified
        /// </summary>
        /// <param name="player">The player to exclude</param>
        /// <param name="body">The message body</param>
        protected void SmsAllExcept(Player player, string body)
        {
            SmsAllExcept(new Player[] { player }, body);
        }
        
        /// <summary>
        /// Send an SMS to all players in the current game except for the specified
        /// </summary>
        /// <param name="playerId">The player id to exclude</param>
        /// <param name="body">The message body</param>
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