using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text.Json;
using JSONAdminEditor.Application.Models.Configuration;
using JSONAdminEditor.Application.Interfaces;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/customers")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly IFileContentService _fileService;
    private readonly JsonSerializerOptions _jsonOptions;

    public CustomersController(IFileContentService fileService)
    {
        _fileService = fileService;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
    }

    private static bool IsValidCustomerId(string customerId) =>
        !string.IsNullOrWhiteSpace(customerId) && 
        Regex.IsMatch(customerId, @"^[a-zA-Z0-9_-]+$") && 
        customerId.Length <= 50;

    private async Task<CustomerNotificationMapping?> GetCustomerDataAsync(string customerId)
    {
        var content = await _fileService.ReadFileAsync($"data/customers/{customerId}.json");
        return string.IsNullOrEmpty(content) ? null : JsonSerializer.Deserialize<CustomerNotificationMapping>(content, _jsonOptions);
    }

    private async Task SaveCustomerDataAsync(string customerId, CustomerNotificationMapping customerData)
    {
        var json = JsonSerializer.Serialize(customerData, _jsonOptions);
        await _fileService.WriteFileAsync($"data/customers/{customerId}.json", json);
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}")]
    [ProducesResponseType(200, Type = typeof(CustomerNotificationMapping))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CustomerNotificationMapping>> GetCustomerConfig(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData ?? new CustomerNotificationMapping());
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerConfig(string customerId, [FromBody] CustomerNotificationMapping customerData)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");
        
        if (customerData == null)
            return BadRequest("Customer data is required");
        
        await SaveCustomerDataAsync(customerId, customerData);
        return NoContent();
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/preferred-communication")]
    [ProducesResponseType(200, Type = typeof(List<PreferredCommunication>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<PreferredCommunication>>> GetCustomerPreferredCommunication(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData?.PreferredCommunication ?? new List<PreferredCommunication>());
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/preferred-communication")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerPreferredCommunication(string customerId, [FromBody] List<PreferredCommunication> data)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");
        
        if (data == null)
            return BadRequest("Preferred communication data is required");
        
        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid preferred communication data");
        
        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");
        
        customerData.PreferredCommunication = data;
        await SaveCustomerDataAsync(customerId, customerData);
        return NoContent();
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/content-variables")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, string>>> GetCustomerContentVariables(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData?.ContentVariables ?? new Dictionary<string, string>());
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/content-variables")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerContentVariables(string customerId, [FromBody] Dictionary<string, string> contentVariables)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");
        
        if (contentVariables == null)
            return BadRequest("Content variables data is required");
        
        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");
        
        customerData.ContentVariables = contentVariables;
        await SaveCustomerDataAsync(customerId, customerData);
        return NoContent();
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/after-hours")]
    [ProducesResponseType(200, Type = typeof(AfterHours))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AfterHours?>> GetCustomerAfterHours(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData?.AfterHours);
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/after-hours")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerAfterHours(string customerId, [FromBody] AfterHours afterHours)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");
        
        if (afterHours == null)
            return BadRequest("After hours data is required");
        
        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid after hours data");
        
        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");
        
        customerData.AfterHours = afterHours;
        await SaveCustomerDataAsync(customerId, customerData);
        return NoContent();
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/from-email")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string?>> GetCustomerFromEmail(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData?.FromEmail ?? "");
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/from-email")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerFromEmail(string customerId, [FromBody] string fromEmail)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");
        
        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");
        
        customerData.FromEmail = fromEmail;
        await SaveCustomerDataAsync(customerId, customerData);
        return NoContent();
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/content-variables-overrides")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, Dictionary<string, Dictionary<string, string>>>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> GetCustomerContentVariablesOverrides(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData?.ContentVariablesOverrides ?? new Dictionary<string, Dictionary<string, Dictionary<string, string>>>());
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/content-variables-overrides")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerContentVariablesOverrides(string customerId, [FromBody] Dictionary<string, Dictionary<string, Dictionary<string, string>>> contentVariablesOverrides)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");
        
        if (contentVariablesOverrides == null)
            return BadRequest("Content variables overrides data is required");
        
        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");
        
        customerData.ContentVariablesOverrides = contentVariablesOverrides;
        await SaveCustomerDataAsync(customerId, customerData);
        return NoContent();
    }
}