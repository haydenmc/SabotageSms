using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SabotageSms.Providers
{
    public class PlivoSmsProvider : ISmsProvider
    {
        private readonly string MessageUrl = "https://api.plivo.com/v1/Account/{0}/Message/";
        
        private IConfigurationRoot Configuration { get; set; }
        
        public PlivoSmsProvider(IConfigurationRoot configuration) {
            Configuration = configuration;
        }
        
        public async Task SendSms(string phoneNumber, string body)
        {
            var messageUrl = String.Format(MessageUrl, Configuration["ApiKeys:PlivoAuthId"]);
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.Credentials
                = new System.Net.NetworkCredential(
                    Configuration["ApiKeys:PlivoAuthId"],
                    Configuration["ApiKeys:PlivoAuthToken"]);
            using (var client = new HttpClient(httpClientHandler)) {
                var postData = new {
                    src = Configuration["ApiKeys:PlivoSourceNumber"],
                    dst = phoneNumber,
                    text = body
                };
                var postString = JsonConvert.SerializeObject(postData);
                var result = await client.PostAsync(messageUrl,
                    new StringContent(postString, Encoding.UTF8, "application/json"));
                if (!result.IsSuccessStatusCode) {
                    var resultStr = await result.Content.ReadAsStringAsync();
                    throw new Exception("Failed to send SMS via Plivo provider: " + resultStr);
                }
            }
        }
    }
}