using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using JSONAdminEditor.Application.Models.Configuration;
using JSONAdminEditor.Services;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/customers/{customerId}/events")]
[Produces("application/json")]
public class CustomerEventsController(IJsonFileService jsonFileService) : ControllerBase
{
    private readonly IJsonFileService _jsonFileService = jsonFileService;

    private static bool IsValidCustomerId(string customerId) =>
        !string.IsNullOrWhiteSpace(customerId) && 
        Regex.IsMatch(customerId, @"^[a-zA-Z0-9_-]+$") && 
        customerId.Length <= 50;

    private async Task<CustomerNotificationMapping?> GetCustomerDataAsync(string customerId)
    {
        return await _jsonFileService.LoadJsonFileAsync<CustomerNotificationMapping>($"data/customers/{customerId}.json");
    }

    private CustomerEventMapping? FindEventMapping(CustomerNotificationMapping customerData, string eventName, string orderType) =>
        customerData.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<CustomerEventMapping>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<CustomerEventMapping>>> GetCustomerEvents(string customerId)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        return Ok(customerData?.EventMappings ?? []);
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(CustomerEventMapping))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CustomerEventMapping>> GetCustomerEvent(string customerId, string eventName, string orderType)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        var eventMapping = customerData != null ? FindEventMapping(customerData, eventName, orderType) : null;

        return Ok(eventMapping ?? new CustomerEventMapping { Event = eventName, OrderType = orderType });
    }

    [HttpPut("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomerEvent(string customerId, string eventName, string orderType, [FromBody] CustomerEventMapping eventMapping)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        if (eventMapping == null)
            return BadRequest("Event mapping data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid event mapping data");

        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");

        eventMapping.Event = eventName;
        eventMapping.OrderType = orderType;

        var existingIndex = customerData.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
            customerData.EventMappings[existingIndex] = eventMapping;
        else
            customerData.EventMappings.Add(eventMapping);

        await _jsonFileService.SaveJsonFileAsync($"data/customers/{customerId}.json", customerData);
        return NoContent();
    }

    [HttpDelete("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteCustomerEvent(string customerId, string eventName, string orderType)
    {
        if (!IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await GetCustomerDataAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");

        var existingIndex = customerData.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (existingIndex < 0)
            return NotFound("Event mapping not found");

        customerData.EventMappings.RemoveAt(existingIndex);
        await _jsonFileService.SaveJsonFileAsync($"data/customers/{customerId}.json", customerData);
        return NoContent();
    }
}