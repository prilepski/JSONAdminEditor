using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/content-variables")]
public class ContentVariablesController : ControllerBase
{
    private readonly NotificationsService _notificationsService;
    private readonly IMockDatabaseService _mockDb;

    public ContentVariablesController(NotificationsService notificationsService, IMockDatabaseService mockDb)
    {
        _notificationsService = notificationsService;
        _mockDb = mockDb;
    }

    [HttpGet]
    public async Task<IActionResult> GetContentVariables()
    {
        var data = await _mockDb.GetContentVariablesAsync();
        var listData = data.Select(kvp => new Dictionary<string, object> { ["key"] = kvp.Key, ["value"] = kvp.Value }).ToList();
        return Ok(listData);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveContentVariables([FromBody] Dictionary<string, string> contentVariables)
    {
        if (contentVariables == null)
            return BadRequest(new { success = false, error = "Content variables data is required" });
            
        var success = await _mockDb.SaveContentVariablesAsync(contentVariables);
        
        if (success)
        {
            return Ok(new { success = true, message = "Content Variables saved successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save content variables" });
        }
    }
    

}