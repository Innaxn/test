using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using OrderMonitoring.Model;
using OrderMonitoring.Controllers;
using Azure.Core;

namespace OrderMonitoring.Infrastructure
{
    public class TeamsAlertChannel: IAlertChannel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _webhookUrl;

        public TeamsAlertChannel(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;
            _webhookUrl = Environment.GetEnvironmentVariable("WebhookUrl")!;
        }

        public async Task SendAlertAsync(AlertMessage message)
        {
            // create HTTP client
            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                title = message.Content,
                time = message.Timestamp,
                navigationLink = message.Severity
            };

            // serialize the payload
            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_webhookUrl, content);

        }
      
    }     
}
