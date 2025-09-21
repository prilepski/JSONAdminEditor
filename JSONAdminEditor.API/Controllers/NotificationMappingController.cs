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
        try
        {
            var data = await _mockDb.GetGlobalDataAsync("notification-mapping");
            return Ok(data?["data"] ?? new NotificationMapping());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving notification mapping" });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveNotificationMapping([FromBody] NotificationMapping notificationMapping)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, error = "Invalid notification mapping data" });

            var success = await _mockDb.SaveGlobalDataAsync("notification-mapping", new Dictionary<string, object> { ["data"] = notificationMapping });

            return Ok(ApiResponse<string>.SuccessResult("Notification mapping saved successfully!"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving notification mapping" });
        }
    }
}