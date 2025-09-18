using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/content-variables")]
public class ContentVariablesController : ControllerBase
{
    private readonly NotificationsService _notificationsService;

    public ContentVariablesController(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetContentVariables()
    {
        try
        {
            var data = await _notificationsService.GetContentVariablesAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveContentVariables([FromBody] List<Dictionary<string, object>> data)
    {
        try
        {
            var success = await _notificationsService.UpdateContentVariablesAsync(data);
            
            if (success)
            {
                return Ok(new { success = true, message = "Content Variables saved successfully!" });
            }
            else
            {
                return Ok(new { success = false, error = "Failed to save content variables" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}