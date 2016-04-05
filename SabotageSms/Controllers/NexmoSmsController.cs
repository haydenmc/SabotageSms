using Microsoft.AspNet.Mvc;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.Controllers
{
    /// <summary>
    /// Nexmo implementation of incoming SMS
    /// https://docs.nexmo.com/api-ref/sms-api
    /// </summary>
    public class NexmoSmsController : SmsController
    {
        public NexmoSmsController(IGameDataProvider gameDataProvider, ParsingProvider parsingProvider, ISmsProvider smsProvider)
            : base(gameDataProvider, parsingProvider, smsProvider) {}
        
        [HttpPost]
        [Route("")]
        public IActionResult ReceiveSms(NexmoIncomingMessageModel incomingSms)
        {
            ProcessSms(incomingSms.Msisdn, incomingSms.Text);
            return Ok();
        }
    }
}