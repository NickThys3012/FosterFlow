using System.Globalization;
using Microsoft.AspNetCore.Components;
namespace FosterFlow.Web.Components;

public partial class FosterFlowDateInput : ComponentBase
{

    private static readonly string[] WeekdayHeaders =
    {
        "Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"
    };

    private readonly string _id = $"date-{Guid.NewGuid():N}";
    private bool _open;
    private DateOnly _viewMonth;
    [Parameter] public DateOnly? Value { get; set; }
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = "ti-calendar";
    [Parameter] public string Placeholder { get; set; } = "Pick a date";
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Hint { get; set; }
    [Parameter] public string? ErrorMessage { get; set; }
    [Parameter] public DateOnly? Min { get; set; }
    [Parameter] public DateOnly? Max { get; set; }

    private bool ShowError => !string.IsNullOrEmpty(ErrorMessage);
    private string ValidationCssClass => ShowError ? "modified invalid" : string.Empty;

    private string DisplayValue =>
        Value?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? string.Empty;

    private string MonthLabel =>
        _viewMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    protected override void OnParametersSet()
    {
        // Keep the visible month in sync with the selected value while closed.
        if (!_open)
        {
            _viewMonth = FirstOfMonth(Value ?? Min ?? Today);
        }
    }

    private static DateOnly FirstOfMonth(DateOnly d)
    {
        return new DateOnly(d.Year, d.Month, 1);
    }

    private void ToggleOpen()
    {
        if (Disabled)
        {
            return;
        }
        _open = !_open;
        if (_open)
        {
            _viewMonth = FirstOfMonth(Value ?? Min ?? Today);
        }
    }

    private void Close()
    {
        _open = false;
    }

    private void PrevMonth()
    {
        _viewMonth = _viewMonth.AddMonths(-1);
    }

    private void NextMonth()
    {
        _viewMonth = _viewMonth.AddMonths(1);
    }

    // 42 days (6 weeks) starting on the Monday on/before the 1st of the view month.
    private IEnumerable<DateOnly> CalendarDays()
    {
        var first = _viewMonth;
        var offset = ((int)first.DayOfWeek + 6) % 7; // Monday = 0
        var start = first.AddDays(-offset);
        for (var i = 0; i < 42; i++)
        {
            yield return start.AddDays(i);
        }
    }

    private bool IsCurrentMonth(DateOnly d)
    {
        return d.Month == _viewMonth.Month && d.Year == _viewMonth.Year;
    }

    private bool IsSelected(DateOnly d)
    {
        return Value == d;
    }

    private static bool IsToday(DateOnly d)
    {
        return d == Today;
    }

    private bool IsDisabled(DateOnly d)
    {
        return (Min.HasValue && d < Min.Value) || (Max.HasValue && d > Max.Value);
    }

    private string DayCssClass(DateOnly d)
    {
        var classes = new List<string>
        {
            "ff-cal-day"
        };
        if (!IsCurrentMonth(d))
        {
            classes.Add("muted");
        }
        if (IsSelected(d))
        {
            classes.Add("selected");
        }
        else if (IsToday(d))
        {
            classes.Add("today");
        }
        if (IsDisabled(d))
        {
            classes.Add("disabled");
        }
        return string.Join(' ', classes);
    }

    private async Task SelectDay(DateOnly d)
    {
        if (IsDisabled(d))
        {
            return;
        }
        _open = false;
        await ValueChanged.InvokeAsync(d);
    }
}
