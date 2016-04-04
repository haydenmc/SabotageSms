using Microsoft.AspNet.Mvc;
using SabotageSms.GameControl;
using SabotageSms.Providers;

namespace SabotageSms.Controllers
{
    /// <summary>
    /// The base class for all incoming SMS controllers.
    /// </summary>
    public abstract class SmsController : Controller
    {
        protected IGameDataProvider _gameDataProvider { get; set; }
        protected ParsingProvider _parsingProvider { get; set; }
        protected ISmsProvider _smsProvider { get; set; }
        
        public SmsController(IGameDataProvider gameDataProvider, ParsingProvider parsingProvider, ISmsProvider smsProvider) {
            _gameDataProvider = gameDataProvider;
            _parsingProvider = parsingProvider;
            _smsProvider = smsProvider;
        }
        
        /// <summary>
        /// Process an SMS and send it to the game manager.
        /// </summary>
        /// <param name="fromNumber">Phone number of the sender</param>
        /// <param name="body">Message body</param>
        [NonAction]
        protected void ProcessSms(string fromNumber, string body)
        {
            // Look up the player
            var player = _gameDataProvider.GetOrCreatePlayerByPhoneNumber(fromNumber);
            var parsedCommand = _parsingProvider.ParseCommand(player, body);

            // Pass their command to game manager
            var game = _gameDataProvider.GetPlayerCurrentGame(player.PlayerId);
            var gameManager = new GameManager(game, _gameDataProvider, _smsProvider);
            gameManager.ExecuteCommand(player, parsedCommand.Command, parsedCommand.Parameters);
        }
    }
}