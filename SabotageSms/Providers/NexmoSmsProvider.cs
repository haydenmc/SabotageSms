using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        
        public NexmoSmsProvider(IConfigurationRoot configuration)
        {
            _configuration = configuration;
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
                if (!result.IsSuccessStatusCode) {
                    var resultStr = await result.Content.ReadAsStringAsync();
                    throw new Exception("Failed to send SMS via Nexmo provider: " + resultStr);
                }
            }
        }
    }
}