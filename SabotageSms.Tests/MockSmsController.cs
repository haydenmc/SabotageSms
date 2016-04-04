using SabotageSms.Controllers;
using SabotageSms.Providers;

namespace SabotageSms.Tests
{
    class MockSmsController : SmsController
    {
        public MockSmsController(IGameDataProvider gameDataProvider, ParsingProvider parsingProvider, ISmsProvider smsProvider)
            : base(gameDataProvider, parsingProvider, smsProvider) {}
        
        public void Sms(string fromNumber, string body)
        {
            ProcessSms(fromNumber, body);
        }
    }
}