using Client_API.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromQuery] string header, [FromQuery] string message)
        {
            await _hubContext.Clients.All.SendAsync("NOTIFICATIONS",
                System.Text.Json.JsonSerializer.Serialize(new { Header = header, Message = message }));
            return Ok("Notification sent.");
        }
    }
}
