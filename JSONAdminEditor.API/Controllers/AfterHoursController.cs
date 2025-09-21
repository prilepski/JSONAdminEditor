using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/afterhours")]
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
        try
        {
            var data = await _mockDb.GetGlobalDataAsync("afterhours");
            return Ok(data?["data"] ?? null);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving after hours settings" });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveAfterHours([FromBody] AfterHours afterHours)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, error = "Invalid after hours data" });

            var success = await _mockDb.SaveGlobalDataAsync("afterhours", new Dictionary<string, object> { ["data"] = afterHours });

            return Ok(ApiResponse<string>.SuccessResult("After hours settings saved successfully!"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving after hours settings" });
        }
    }
}