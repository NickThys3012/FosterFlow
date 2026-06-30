namespace FosterFlow.Shared;

public static class EnumUtils
{
    public static T ToEnum<T>(this string value, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
    }
}
