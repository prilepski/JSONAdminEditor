using JSONAdminEditor.Application.Models;
using System.Text.Json;

namespace JSONAdminEditor.Services;

public interface IBackwardCompatibilityService
{
    Dictionary<string, object> EnsureCompatibility(Dictionary<string, object> data, Type targetType);
    T? SafeDeserialize<T>(object? data) where T : class;
}

public class BackwardCompatibilityService : IBackwardCompatibilityService
{
    public Dictionary<string, object> EnsureCompatibility(Dictionary<string, object> data, Type targetType)
    {
        if (targetType == typeof(EventMapping))
        {
            return EnsureEventMappingCompatibility(data);
        }
        if (targetType == typeof(AfterHours))
        {
            return EnsureAfterHoursCompatibility(data);
        }
        if (targetType == typeof(OptOut))
        {
            return EnsureOptOutCompatibility(data);
        }

        return data;
    }

    public T? SafeDeserialize<T>(object? data) where T : class
    {
        if (data == null) return null;

        try
        {
            string json;
            if (data is string str)
            {
                json = str;
            }
            else
            {
                json = JsonSerializer.Serialize(data);
            }
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return null;
        }
    }

    private Dictionary<string, object> EnsureEventMappingCompatibility(Dictionary<string, object> data)
    {
        // Ensure Templates is a proper dictionary with Channel enum keys
        if (data.TryGetValue("Templates", out var templates))
        {
            if (templates is Dictionary<string, object> templatesDict)
            {
                var newTemplates = new Dictionary<string, string>();
                foreach (var kvp in templatesDict)
                {
                    newTemplates[kvp.Key] = kvp.Value?.ToString() ?? "";
                }
                data["Templates"] = newTemplates;
            }
        }

        // Ensure required fields exist
        data.TryAdd("Event", "");
        data.TryAdd("OrderType", "");
        data.TryAdd("Phone", "");
        data.TryAdd("Email", "");
        data.TryAdd("IsSuppressed", false);
        data.TryAdd("PreferredCommunication", new List<object>());
        data.TryAdd("ContentVariables", new Dictionary<string, object>());
        data.TryAdd("TriggerConditions", new Dictionary<string, object>());
        data.TryAdd("ContentVariablesOverrides", new Dictionary<string, object>());

        return data;
    }

    private Dictionary<string, object> EnsureAfterHoursCompatibility(Dictionary<string, object> data)
    {
        // Ensure RestrictedHoursPeriod exists
        if (!data.ContainsKey("RestrictedHoursPeriod"))
        {
            data["RestrictedHoursPeriod"] = new Dictionary<string, object>
            {
                ["Start"] = "22:00",
                ["End"] = "08:00"
            };
        }

        return data;
    }

    private Dictionary<string, object> EnsureOptOutCompatibility(Dictionary<string, object> data)
    {
        // OptOut is a dictionary structure, ensure it's properly formatted
        return data;
    }
}