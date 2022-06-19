namespace Totoro.Extensions;

public static class DateTimeExtensions
{
    public static DateTime NearestDate(this DateTime datetime, IEnumerable<DateTime> datetimes)
    {
        return datetime.Add(datetimes.Min(value => (value - datetime).Duration()));
    }
}
