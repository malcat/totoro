using System.Text.RegularExpressions;

namespace Totoro.Extensions;

public static class StringExtensions
{
    public static Guid ToGuid(this string value)
    {
        return Guid.TryParse(value, out var guid) ? guid : default;
    }

    public static bool Equivalent(this string value, string target)
    {
        var equivalent = Regex.Replace(value, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);

        return equivalent.Equals(target, StringComparison.OrdinalIgnoreCase);
    }
}
