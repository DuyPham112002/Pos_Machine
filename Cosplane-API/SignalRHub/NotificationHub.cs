using Microsoft.AspNetCore.SignalR;

namespace Cosplane_API.SignalRHub
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string header, string message)
        {
            var notification = new { Header = header, Message = message };
            var notificationJson = System.Text.Json.JsonSerializer.Serialize(notification);
            await Clients.All.SendAsync("NOTIFICATIONS", notificationJson);
        }
    }
}
