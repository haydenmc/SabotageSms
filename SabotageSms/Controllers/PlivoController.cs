using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
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
        
        // GET: api/values
        [HttpGet]
        [HttpPost]
        public async Task<PlivoResponseModel> ReceiveSms(
            [FromQuery] string From,
            [FromQuery] string To,
            [FromQuery] string Type,
            [FromQuery] string Text,
            [FromQuery] string MessageUUID)
        {
            try {
                await _smsProvider.SendSms(From, "We got your text!\n" + Text);
                return new PlivoResponseModel();
            } catch (Exception) {
                return new PlivoResponseModel();
            }
        }
    }
}
