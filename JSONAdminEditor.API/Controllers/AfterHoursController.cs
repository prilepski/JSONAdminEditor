using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/after-hours")]
public class AfterHoursController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public AfterHoursController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    [HttpGet]
    public async Task<IActionResult> GetAfterHours()
    {
        var afterHours = await _mockDb.GetAfterHoursAsync();
        return Ok(afterHours);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveAfterHours([FromBody] AfterHours afterHours)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid after hours data" });

        var success = await _mockDb.SaveAfterHoursAsync(afterHours);
        
        if (success)
        {
            return Ok(new { success = true, message = "After hours settings saved successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save after hours settings" });
        }
    }
}