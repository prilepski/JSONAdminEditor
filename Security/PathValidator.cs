using System.IO;

namespace JSONAdminEditor.Security;

public static class PathValidator
{
    private static readonly string[] AllowedPaths = {
        "Data/Templates.json",
        "Data/EventTriggers.json", 
        "Data/EventChannels.json",
        "Data/OrderTypes.json",
        "Data/Customers.json",
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