using JSONAdminEditor.Application.Models;
using System.Text.Json;

namespace JSONAdminEditor.Services;

public interface IDataMigrationService
{
    T? MigrateToModel<T>(Dictionary<string, object>? data) where T : class;
    Dictionary<string, object> MigrateFromModel<T>(T model) where T : class;
    bool ValidateDataStructure<T>(Dictionary<string, object>? data) where T : class;
}

public class DataMigrationService : IDataMigrationService
{
    public T? MigrateToModel<T>(Dictionary<string, object>? data) where T : class
    {
        if (data == null) return null;

        try
        {
            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return null;
        }
    }

    public Dictionary<string, object> MigrateFromModel<T>(T model) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(model);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }

    public bool ValidateDataStructure<T>(Dictionary<string, object>? data) where T : class
    {
        if (data == null) return false;

        try
        {
            var migrated = MigrateToModel<T>(data);
            return migrated != null;
        }
        catch
        {
            return false;
        }
    }
}