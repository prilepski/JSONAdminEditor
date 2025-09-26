using System.Text.RegularExpressions;

namespace JSONAdminEditor.Services;

public static class Validator
{
    public static bool HasUniqueKeys<T, TKey>(List<T> items, Func<T, TKey> keySelector) => 
        items.GroupBy(keySelector)
            .Where(group => group.Count() > 1)
            .Any();

    public static bool IsValidCustomerId(string customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        Regex.IsMatch(customerId, @"^[a-zA-Z0-9_-]+$") &&
        customerId.Length <= 50;
}
