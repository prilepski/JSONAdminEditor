namespace JSONAdminEditor.Validators;

public static class PathValidator
{
    private static readonly string[] AllowedPaths = {
        "data/dictionaries/templates.json",
        "data/dictionaries/event-triggers.json", 
        "data/dictionaries/event-channels.json",
        "data/dictionaries/order-types.json",
        "data/dictionaries/customers.json",
        "Data/ContentVariables.json",
        "Data/PreferredCommunication.json"
    };

    public static bool IsValidPath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var normalizedPath = Path.GetFullPath(filePath).Replace('\\', '/');
        
        return AllowedPaths.Any(allowed => 
            normalizedPath.EndsWith(allowed, StringComparison.OrdinalIgnoreCase));
    }

    public static string SanitizePath(string filePath)
    {
        if (!IsValidPath(filePath))
            throw new UnauthorizedAccessException("Invalid file path");
            
        return filePath;
    }
}