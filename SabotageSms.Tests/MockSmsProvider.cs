using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SabotageSms.Providers;
using Xunit.Abstractions;

namespace SabotageSms.Tests
{
    class MockSmsProvider : ISmsProvider
    {
        private ITestOutputHelper _output;
        
        public MockSmsProvider(ITestOutputHelper output)
        {
            _output = output;
        }
        
        public async Task SendSms(string phoneNumber, string body)
        {
            _output.WriteLine("[SMS] {0}: {1}", phoneNumber, body);
        }
    }
}