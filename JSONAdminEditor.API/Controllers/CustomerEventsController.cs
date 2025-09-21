using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using System.Text.RegularExpressions;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/customers/{customerId}/events")]
[Produces("application/json")]
public class CustomerEventsController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public CustomerEventsController(IMockDatabaseService mockDb)
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
    [ProducesResponseType(200, Type = typeof(List<EventMapping>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventMapping>>> GetCustomerEvents(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });

        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { error = "Customer not found" });

        return Ok(customerData.EventMappings);
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(EventMapping))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EventMapping?>> GetCustomerEvent(string customerId, string eventName, string orderType)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest(new { error = "Invalid customer ID" });

        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { error = "Customer not found" });

        var eventMapping = customerData.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (eventMapping == null)
            return NotFound(new { error = "Event mapping not found" });

        return Ok(eventMapping);
    }

    [HttpPut("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerEvent(string customerId, string eventName, string orderType, [FromBody] EventMapping eventMapping)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });

        if (eventMapping == null)
            return BadRequest(new { success = false, error = "Event mapping data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid event mapping data" });

        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { success = false, error = "Customer not found" });

        // Ensure the event mapping has correct event name and order type
        eventMapping.Event = eventName;
        eventMapping.OrderType = orderType;

        var existingIndex = customerData.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            customerData.EventMappings[existingIndex] = eventMapping;
        }
        else
        {
            customerData.EventMappings.Add(eventMapping);
        }

        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);

        if (success)
        {
            return Ok(new { success = true, message = "Customer event mapping updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update customer event mapping" });
        }
    }

    [HttpDelete("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteCustomerEvent(string customerId, string eventName, string orderType)
    {
        if (!IsValidCustomerId(customerId))
            return UnprocessableEntity(new { success = false, error = "Invalid customer ID" });

        var customerData = await _mockDb.GetCustomerAsync(customerId);
        if (customerData == null)
            return NotFound(new { success = false, error = "Customer not found" });

        var existingIndex = customerData.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (existingIndex < 0)
            return NotFound(new { success = false, error = "Event mapping not found" });

        customerData.EventMappings.RemoveAt(existingIndex);
        var success = await _mockDb.SaveCustomerAsync(customerId, customerData);

        if (success)
        {
            return Ok(new { success = true, message = "Customer event mapping deleted successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to delete customer event mapping" });
        }
    }
}