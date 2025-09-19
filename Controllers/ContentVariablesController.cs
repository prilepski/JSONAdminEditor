using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using System.Text.Json;

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
            return StatusCode(500, new { error = "An error occurred while retrieving content variables" });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveContentVariables([FromBody] JsonElement requestData)
    {
        try
        {
            var dataProperty = requestData.GetProperty("data");
            var tableData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(dataProperty.GetRawText());
            var convertedData = tableData?.Select(row => 
                row.ToDictionary(kvp => kvp.Key, kvp => GetJsonElementValue(kvp.Value))
            ).ToList();
            
            var success = await _notificationsService.UpdateContentVariablesAsync(convertedData);
            
            if (success)
            {
                return Ok(new { success = true, message = "Content Variables saved successfully!" });
            }
            else
            {
                return BadRequest(new { success = false, error = "Failed to save content variables" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving content variables" });
        }
    }
    
    private static object GetJsonElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}