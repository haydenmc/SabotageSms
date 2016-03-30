using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using SabotageSms.GameControl;
using SabotageSms.Models;
using SabotageSms.Models.Plivo;
using SabotageSms.Providers;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SabotageSms.Controllers
{
    [Route("api/Plivo")]
    public class PlivoController : Controller
    {
        private IGameDataProvider _gameDataProvider { get; set; }
        private ParsingProvider _parsingProvider { get; set; }
        private ISmsProvider _smsProvider { get; set; }
        private IConfigurationRoot _configuration { get; set; }
        
        public PlivoController(IGameDataProvider gameDataProvider, ParsingProvider parsingProvider, ISmsProvider smsProvider, IConfigurationRoot configuration) {
            _gameDataProvider = gameDataProvider;
            _parsingProvider = parsingProvider;
            _smsProvider = smsProvider;
            _configuration = configuration;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<PlivoResponseModel> ReceiveSms(PlivoIncomingMessageModel incomingSms)
        {
            // Look up the player
            var player = _gameDataProvider.GetOrCreatePlayerByPhoneNumber(incomingSms.From);
            var parsedCommand = _parsingProvider.ParseCommand(player, incomingSms.Text);
            
            // TODO: This shouldn't be the SMS controller's responsibility...
            if (parsedCommand.Command == Command.Name)
            {
                var requestedName = parsedCommand.Parameters as string;
                var assignedName = new Regex("[^a-zA-Z]").Replace(requestedName, "");
                if (assignedName.Length > GameManager.MaxNameLength) {
                    assignedName = assignedName.Substring(0, GameManager.MaxNameLength);
                }
                _gameDataProvider.SetPlayerName(player.PlayerId, assignedName);
                await _smsProvider.SendSms(player.PhoneNumber,
                    String.Format("Your name has been set to '{0}'. 'Start' or 'Join CODEHERE'.", assignedName));
                return new PlivoResponseModel();
            }
            // If they don't have a name, we need to ask them to set one
            // TODO: This shouldn't be the SMS controller's responsibility...
            if (player.Name == null || player.Name.Length <= 0)
            {
                await _smsProvider.SendSms(player.PhoneNumber,
                    "Hi! Before you start, you need to set a name. Please reply with 'Name YOURNAMEHERE'.");
                return new PlivoResponseModel();
            }
            
            // Pass their command to game manager
            var game = _gameDataProvider.GetPlayerCurrentGame(player.PlayerId);
            var gameManager = new GameManager(game, _gameDataProvider, _smsProvider);
            gameManager.ExecuteCommand(player, parsedCommand.Command, parsedCommand.Parameters);
            return new PlivoResponseModel();
        }
    }
}
