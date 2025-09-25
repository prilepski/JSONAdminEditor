using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Services;

public class UniqueFieldValidationService
{
    private readonly Dictionary<string, string> _uniqueFieldMap;
    private readonly IWebHostEnvironment _environment;

    public UniqueFieldValidationService(IWebHostEnvironment environment)
    {
        _environment = environment;
        // Define the unique field for each dictionary file
        _uniqueFieldMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "templates.json", "templateId" },
            { "customers.json", "customerId" },
            { "event-triggers.json", "Event Name" },
            { "event-channels.json", "Channel Name" },
            { "order-types.json", "Order Type" }
        };
    }

    public string? GetUniqueField(string fileName)
    {
        return _uniqueFieldMap.TryGetValue(fileName, out var field) ? field : null;
    }

    public bool IsDictionaryFile(string fileName)
    {
        return _uniqueFieldMap.ContainsKey(fileName);
    }

    private async Task<bool> IsValidChannelAsync(string channelType)
    {
        try
        {
            var channelsFilePath = Path.Combine(_environment.WebRootPath, "data", "dictionaries", "event-channels.json");
            
            if (!File.Exists(channelsFilePath))
            {
                return false;
            }

            var jsonContent = await File.ReadAllTextAsync(channelsFilePath);
            var channels = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
            
            if (channels == null) return false;
            
            return channels.Any(c => 
                c.TryGetValue("Channel Name", out var nameObj) && 
                nameObj?.ToString()?.Equals(channelType, StringComparison.OrdinalIgnoreCase) == true &&
                c.TryGetValue("IsActive", out var activeObj) &&
                (activeObj is bool isActive ? isActive : activeObj?.ToString()?.ToLower() == "true"));
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<ValidationResult> ValidateUniquenessAsync(string fileName, List<Dictionary<string, object>> data)
    {
        var result = new ValidationResult();
        
        if (!IsDictionaryFile(fileName))
        {
            return result; // No validation needed for non-dictionary files
        }

        var uniqueField = GetUniqueField(fileName);
        if (string.IsNullOrEmpty(uniqueField))
        {
            return result;
        }

        var seenValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicateValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // First pass: identify duplicates and validate required fields
        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            
            // Special validation for templates.json - channelType is required
            if (fileName.Equals("templates.json", StringComparison.OrdinalIgnoreCase))
            {
                if (!row.TryGetValue("channelType", out var channelTypeObj) || 
                    string.IsNullOrWhiteSpace(channelTypeObj?.ToString()))
                {
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = "channelType",
                        Message = "Channel Type is required"
                    });
                }
                else
                {
                    // Validate that channelType exists in event-channels.json
                    var isValidChannel = await IsValidChannelAsync(channelTypeObj.ToString()!);
                    if (!isValidChannel)
                    {
                        result.Errors.Add(new ValidationError
                        {
                            RowIndex = i,
                            FieldName = "channelType",
                            Message = $"Channel Type '{channelTypeObj}' does not exist in Event Channels dictionary or is inactive"
                        });
                    }
                }
            }
            
            // Existing unique field validation
            if (row.TryGetValue(uniqueField, out var valueObj))
            {
                var value = valueObj?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!seenValues.Add(value))
                    {
                        duplicateValues.Add(value);
                    }
                }
                else
                {
                    // Empty or null values are also invalid
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = uniqueField,
                        Message = $"{uniqueField} cannot be empty"
                    });
                }
            }
            else
            {
                // Missing field
                result.Errors.Add(new ValidationError
                {
                    RowIndex = i,
                    FieldName = uniqueField,
                    Message = $"{uniqueField} is required"
                });
            }
        }

        // Second pass: mark all rows with duplicate values
        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            if (row.TryGetValue(uniqueField, out var valueObj))
            {
                var value = valueObj?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(value) && duplicateValues.Contains(value))
                {
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = uniqueField,
                        Message = $"Duplicate {uniqueField}: '{value}' must be unique"
                    });
                }
            }
        }

        result.IsValid = !result.Errors.Any();
        return result;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<ValidationError> Errors { get; set; } = new();
}
