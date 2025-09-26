namespace JSONAdminEditor.Services;

public static class Validator
{
    public static bool HasUniqueKeys<T, TKey>(List<T> items, Func<T, TKey> keySelector)
    {
        return items.GroupBy(keySelector)
            .Where(group => group.Count() > 1)
            .Any();
    }
}
