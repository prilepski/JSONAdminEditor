using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/optout")]
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
        try
        {
            var data = await _mockDb.GetGlobalDataAsync("optout");
            var optOut = data?["data"] ?? new OptOut();
            return Ok(optOut);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving opt-out settings" });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveOptOut([FromBody] OptOut optOut)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, error = "Invalid opt-out data" });

            var success = await _mockDb.SaveGlobalDataAsync("optout", new Dictionary<string, object> { ["data"] = optOut });

            return Ok(ApiResponse.Success("Opt-out settings saved successfully!"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving opt-out settings" });
        }
    }
}