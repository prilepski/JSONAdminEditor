using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Application.Models.Configuration;
using JSONAdminEditor.Services;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/customers/{customerId}/events")]
[Produces("application/json")]
public class CustomerEventsController(IConfigRepository repository) : ControllerBase
{
    private readonly IConfigRepository _repository = repository;

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<CustomerEventMapping>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<CustomerEventMapping>>> GetCustomerEvents(string customerId)
    {
        if (!Validator.IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await _repository.GetCustomerConfigAsync(customerId);
        return Ok(customerData?.EventMappings ?? []);
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(CustomerEventMapping))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CustomerEventMapping>> GetCustomerEvent(string customerId, string eventName, string orderType)
    {
        if (!Validator.IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await _repository.GetCustomerConfigAsync(customerId);
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
        if (!Validator.IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        if (eventMapping == null)
            return BadRequest("Event mapping data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid event mapping data");

        var customerData = await _repository.GetCustomerConfigAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");

        eventMapping.Event = eventName;
        eventMapping.OrderType = orderType;
        int existingIndex = FindIndex(customerData, eventName, orderType);

        if (existingIndex >= 0)
            customerData.EventMappings[existingIndex] = eventMapping;
        else
            customerData.EventMappings.Add(eventMapping);

        await _repository.SaveCustomerConfigAsync(customerId, customerData);
        return NoContent();
    }

    [HttpDelete("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteCustomerEvent(string customerId, string eventName, string orderType)
    {
        if (!Validator.IsValidCustomerId(customerId))
            return BadRequest("Invalid customer ID");

        var customerData = await _repository.GetCustomerConfigAsync(customerId);
        if (customerData == null)
            return NotFound("Customer not found");

        int existingIndex = FindIndex(customerData, eventName, orderType);

        if (existingIndex < 0)
            return NotFound("Event mapping not found");

        customerData.EventMappings.RemoveAt(existingIndex);
        await _repository.SaveCustomerConfigAsync(customerId, customerData);
        return NoContent();
    }


    private static CustomerEventMapping? FindEventMapping(CustomerNotificationMapping customerData, string eventName, string orderType) =>
        customerData.EventMappings.FirstOrDefault(e =>
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

    private static int FindIndex(CustomerNotificationMapping customerData, string eventName, string orderType) =>
        customerData.EventMappings.FindIndex(e =>
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));
}