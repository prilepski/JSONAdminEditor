using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Services
{
    public class UniqueFieldValidationService
    {
        private readonly Dictionary<string, string> _uniqueFieldMap;

        public UniqueFieldValidationService()
        {
            // Define the unique field for each default settings file
            _uniqueFieldMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "templates.json", "templateId" },
                { "customers.json", "customerId" },
                { "event-triggers.json", "Event Name" },
                { "event-channels.json", "Channel Name" }
            };
        }

        public string? GetUniqueField(string fileName)
        {
            return _uniqueFieldMap.TryGetValue(fileName, out var field) ? field : null;
        }

        public bool IsDefaultSettingsFile(string fileName)
        {
            return _uniqueFieldMap.ContainsKey(fileName);
        }

        public ValidationResult ValidateUniqueness(string fileName, List<Dictionary<string, object>> data)
        {
            var result = new ValidationResult();
            
            if (!IsDefaultSettingsFile(fileName))
            {
                return result; // No validation needed for non-default settings files
            }

            var uniqueField = GetUniqueField(fileName);
            if (string.IsNullOrEmpty(uniqueField))
            {
                return result;
            }

            var seenValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var duplicateValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // First pass: identify duplicates
            for (int i = 0; i < data.Count; i++)
            {
                var row = data[i];
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
}
