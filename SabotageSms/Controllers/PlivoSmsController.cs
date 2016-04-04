using Microsoft.AspNet.Mvc;
using SabotageSms.Models;
using SabotageSms.Models.Plivo;
using SabotageSms.Providers;

namespace SabotageSms.Controllers
{
    /// <summary>
    /// Plivo implementation of incoming SMS
    /// https://www.plivo.com/docs/getting-started/receive-an-sms/
    /// </summary>
    [Route("api/Plivo")]
    public class PlivoSmsController : SmsController
    {
        public PlivoSmsController(IGameDataProvider gameDataProvider, ParsingProvider parsingProvider, ISmsProvider smsProvider)
            : base(gameDataProvider, parsingProvider, smsProvider) {}
        
        [HttpPost]
        [Route("")]
        public PlivoResponseModel ReceiveSms(PlivoIncomingMessageModel incomingSms)
        {
            ProcessSms(incomingSms.From, incomingSms.Text);
            return new PlivoResponseModel();
        }
    }
}
