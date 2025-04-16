using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Infrastructure.SignalR.Hubs;

namespace OrderMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationSender _inotificationSender;
        private readonly NotificationSender _notificationSender;


        public NotificationsController(ILogger<NotificationsController> logger, INotificationSender inotificationSender, NotificationSender notificationSender)
        {
            _logger = logger;
            _inotificationSender = inotificationSender;
            _notificationSender = notificationSender;

        }

        //[HttpPost]
        //public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        // Log the incoming request
        //        _logger.LogInformation("Received notification request: {Title}", request.Title);

        //        // Send the notification using the service
        //        var response = await _notificationSender.SendNotificationAsync(request);

        //        // Check if the request was successful
        //        if (response.IsSuccessStatusCode)
        //        {
        //            _logger.LogInformation("Notification sent successfully to {WebhookUrl}", request.WebhookUrl);
        //            return Ok(new { success = true, message = "Notification sent successfully" });
        //        }
        //        else
        //        {
        //            var errorMessage = $"Failed to send notification. Status code: {response.StatusCode}";
        //            _logger.LogError(errorMessage);
        //            return StatusCode((int)response.StatusCode, new { success = false, message = errorMessage });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error sending notification");
        //        return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Log the incoming request
                _logger.LogInformation("Received notification request: {Title}", request.Title);

                // Send the notification using the service
                var response = await _notificationSender.SendNotificationAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Notification sent successfully to {WebhookUrl}", request.WebhookUrl);
                    return Ok(new { success = true, message = "Notification sent successfully" });
                }
                else
                {
                    var errorMessage = $"Failed to send notification. Status code: {response.StatusCode}";
                    _logger.LogError(errorMessage);
                    return StatusCode((int)response.StatusCode, new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
    }

    public class NotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string NavigationLink { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
    }

    public interface INotificationSender
    {
        Task SendNotificationAsync(NotificationRequest notificationRequest);
    }

    public class NotificationSender
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NotificationSender> _logger;

        public NotificationSender(IHttpClientFactory httpClientFactory, ILogger<NotificationSender> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendNotificationAsync(NotificationRequest request)
        {
            // Create HTTP client
            var client = _httpClientFactory.CreateClient();

            // Prepare the payload
            var payload = new
            {
                title = request.Title,
                time = DateTime.UtcNow,
                navigationLink = request.NavigationLink
            };

            // Serialize the payload
            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the payload to the webhook URL
            var response = await client.PostAsync(request.WebhookUrl, content);

            return response;
        }



        //Task INotificationSender.SendNotificationAsync(NotificationRequest notificationRequest)
        //{
        //    // Create HTTP client
        //    var client = _httpClientFactory.CreateClient();

        //    // Prepare the payload
        //    var payload = new
        //    {
        //        title = request.Title,
        //        time = DateTime.UtcNow,
        //        navigationLink = request.NavigationLink
        //    };

        //    // Serialize the payload
        //    var jsonContent = JsonSerializer.Serialize(payload);
        //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //    // Send the payload to the webhook URL
        //    var response = await client.PostAsync(request.WebhookUrl, content);

        //    return response;
        //}
    }

}
