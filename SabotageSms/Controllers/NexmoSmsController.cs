using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.Controllers
{
    /// <summary>
    /// Nexmo implementation of incoming SMS
    /// https://docs.nexmo.com/api-ref/sms-api
    /// </summary>
    [Route("api/Nexmo")]
    public class NexmoSmsController : SmsController
    {
        private ILogger<NexmoSmsController> _logger { get; set; }
        
        public NexmoSmsController(IGameDataProvider gameDataProvider, ParsingProvider parsingProvider, ISmsProvider smsProvider, ILogger<NexmoSmsController> logger)
            : base(gameDataProvider, parsingProvider, smsProvider)
        {
            _logger = logger;
        }
        
        [HttpGet]
        [HttpPost]
        [Route("")]
        public IActionResult ReceiveSms(NexmoIncomingMessageModel incomingSms)
        {
            if (incomingSms.Msisdn == null || incomingSms.Msisdn.Length <= 0)
            {
                _logger.LogInformation($"Received blank request.");
                return Ok();
            }
            _logger.LogInformation($"Received SMS from {incomingSms.Msisdn} to {incomingSms.To}: '{incomingSms.Text}'");
            ProcessSms(incomingSms.Msisdn, incomingSms.Text);
            return Ok();
        }
    }
}