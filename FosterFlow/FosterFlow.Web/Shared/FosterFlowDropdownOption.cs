using System.Text.RegularExpressions;
namespace FosterFlow.Web.Shared;

public partial class FfDropdownOption
{

    private FfDropdownOption(object? value, string label)
    {
        Value = value;
        Label = label;
    }
    public object? Value { get; init; }
    public string Label { get; init; }

    /// <summary>
    ///     Build options from an enum, using the enum member name as value
    ///     and a display-friendly label (spaces inserted before capitals).
    /// </summary>
    public static List<FfDropdownOption> FromEnum<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(e => new FfDropdownOption(e, ToLabel(e.ToString())))
            .ToList();
    }

    /// <summary>
    ///     Build options from a simple string list (value == label).
    /// </summary>
    public static List<FfDropdownOption> FromStrings(IEnumerable<string> items)
    {
        return items.Select(s => new FfDropdownOption(s, s)).ToList();
    }

    private static string ToLabel(string name)
    {
        return MyRegex().Replace(name, " $1");
    }

    [GeneratedRegex("(?<=[a-z])([A-Z])")]
    private static partial Regex MyRegex();
}
