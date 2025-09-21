using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/notification-mapping")]
public class NotificationMappingController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public NotificationMappingController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotificationMapping()
    {
        var notificationMapping = await _mockDb.GetNotificationMappingAsync();
        return Ok(notificationMapping);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveNotificationMapping([FromBody] NotificationMapping notificationMapping)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid notification mapping data" });

        var success = await _mockDb.SaveNotificationMappingAsync(notificationMapping);
        
        if (success)
        {
            return Ok(new { success = true, message = "Notification mapping saved successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save notification mapping" });
        }
    }
}