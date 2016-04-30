using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SabotageSms.Providers
{
    /// <summary>
    /// Nexmo implementation of outgoing SMS messaging
    /// https://docs.nexmo.com/api-ref/sms-api
    /// </summary>
    public class NexmoSmsProvider : ISmsProvider
    {
        private readonly string _messageUrl = "https://rest.nexmo.com/sms/json";
        private IConfigurationRoot _configuration { get; set; }
        private ILogger<NexmoSmsProvider> _logger { get; set; }
        
        public NexmoSmsProvider(IConfigurationRoot configuration, ILogger<NexmoSmsProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task SendSms(string phoneNumber, string body)
        {
            using (var client = new HttpClient()) {
                var postData = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("api_key", _configuration["ApiKeys:NexmoApiKey"]),
                    new KeyValuePair<string, string>("api_secret", _configuration["ApiKeys:NexmoApiSecret"]),
                    new KeyValuePair<string, string>("to", phoneNumber),
                    new KeyValuePair<string, string>("from", _configuration["ApiKeys:NexmoSourceNumber"]),
                    new KeyValuePair<string, string>("text", body)
                });
                var result = await client.PostAsync(_messageUrl, postData);
                var resultStr = await result.Content.ReadAsStringAsync();
                if (!result.IsSuccessStatusCode) {
                    _logger.LogError($"Failed to send SMS via Nexmo provider:\n{resultStr}");
                    throw new Exception("Failed to send SMS via Nexmo provider: " + resultStr);
                }
                else
                {
                    _logger.LogInformation($"Successfully sent SMS via Nexmo provider:\n{resultStr}");
                }
            }
        }
    }
}