using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/opt-out")]
public class OptOutController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public OptOutController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    [HttpGet]
    public async Task<IActionResult> GetOptOut()
    {
        var optOut = await _mockDb.GetOptOutAsync();
        return Ok(optOut);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveOptOut([FromBody] OptOut optOut)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid opt-out data" });

        var success = await _mockDb.SaveOptOutAsync(optOut);
        
        if (success)
        {
            return Ok(new { success = true, message = "Opt-out settings saved successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save opt-out settings" });
        }
    }
}