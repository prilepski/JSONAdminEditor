using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using System.Text.RegularExpressions;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/customers")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public CustomersController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    private static bool IsValidCustomerId(string customerId)
    {
        return !string.IsNullOrWhiteSpace(customerId) && 
               Regex.IsMatch(customerId, @"^[a-zA-Z0-9_-]+$") && 
               customerId.Length <= 50;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<string>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<string>>> GetCustomers()
    {
        var customers = await _mockDb.SearchCustomersAsync("");
        var customerIds = customers.Select(c => c.TryGetValue("customerId", out var id) ? id?.ToString() : "").Where(id => !string.IsNullOrEmpty(id)).ToList();
        return Ok(customerIds);
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, object>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, object>>> GetCustomerConfig(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });
            
        var data = await _mockDb.GetCustomerAsync(customerId);
        if (data == null)
            return NotFound(new { error = "Customer not found" });
            
        return Ok(data);
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerConfig(string customerId, [FromBody] Dictionary<string, object> customerData)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });
        
        if (customerData == null)
            return BadRequest(new { success = false, error = "Customer data is required" });
        
        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
        
        if (success)
        {
            return Ok(new { success = true, message = "Customer configuration updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update customer configuration" });
        }
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/preferred-communication")]
    [ProducesResponseType(200, Type = typeof(List<PreferredCommunication>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<PreferredCommunication>>> GetCustomerPreferredCommunication(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });
            
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { error = "Customer not found" });
            
        if (customerData.TryGetValue("PreferredCommunication", out var prefCommObj) && 
            prefCommObj is List<PreferredCommunication> prefComm)
        {
            return Ok(prefComm);
        }
        
        return Ok(new List<PreferredCommunication>());
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/preferred-communication")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerPreferredCommunication(string customerId, [FromBody] List<PreferredCommunication> data)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });
        
        if (data == null)
            return BadRequest(new { success = false, error = "Preferred communication data is required" });
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid preferred communication data" });
        
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { success = false, error = "Customer not found" });
        
        customerData["PreferredCommunication"] = data;
        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
        
        if (success)
        {
            return Ok(new { success = true, message = "Customer preferred communication updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update customer preferred communication" });
        }
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/content-variables")]
    [ProducesResponseType(200, Type = typeof(List<object>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<object>>> GetCustomerContentVariables(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });
            
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { error = "Customer not found" });
            
        if (customerData.TryGetValue("ContentVariables", out var cvObj) && 
            cvObj is Dictionary<string, string> contentVars)
        {
            var listData = contentVars.Select(kvp => new { key = kvp.Key, value = kvp.Value }).ToList();
            return Ok(listData);
        }
        
        return Ok(new List<object>());
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/content-variables")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerContentVariables(string customerId, [FromBody] Dictionary<string, string> contentVariables)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });
        
        if (contentVariables == null)
            return BadRequest(new { success = false, error = "Content variables data is required" });
        
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { success = false, error = "Customer not found" });
        
        customerData["ContentVariables"] = contentVariables;
        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
        
        if (success)
        {
            return Ok(new { success = true, message = "Customer content variables updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update customer content variables" });
        }
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/after-hours")]
    [ProducesResponseType(200, Type = typeof(AfterHours))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AfterHours?>> GetCustomerAfterHours(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });
            
        var customer = await _mockDb.GetCustomerAsync(customerId);
        if (customer == null)
            return NotFound(new { error = "Customer not found" });
            
        var afterHours = await _mockDb.GetAfterHoursAsync();
        return Ok(afterHours);
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/after-hours")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerAfterHours(string customerId, [FromBody] AfterHours afterHours)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });
        
        if (afterHours == null)
            return BadRequest(new { success = false, error = "After hours data is required" });
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid after hours data" });
        
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { success = false, error = "Customer not found" });
        
        customerData["AfterHours"] = afterHours;
        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
        
        if (success)
        {
            return Ok(new { success = true, message = "Customer after hours settings updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update customer after hours settings" });
        }
    }

    [HttpGet("{customerId:minlength(1):maxlength(50)}/from-email")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string>> GetCustomerFromEmail(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });
            
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { error = "Customer not found" });
            
        var fromEmail = customerData.TryGetValue("FromEmail", out var emailObj) ? emailObj?.ToString() : "";
        return Ok(fromEmail ?? "");
    }

    [HttpPut("{customerId:minlength(1):maxlength(50)}/from-email")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerFromEmail(string customerId, [FromBody] string fromEmail)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });
        
        if (string.IsNullOrEmpty(fromEmail))
            return BadRequest(new { success = false, error = "From email is required" });
        
        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { success = false, error = "Customer not found" });
        
        customerData["FromEmail"] = fromEmail;
        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
        
        if (success)
        {
            return Ok(new { success = true, message = "Customer from email updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update customer from email" });
        }
    }
}