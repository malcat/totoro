using System.Text.Json;

namespace Totoro.Web.Extensions;

public static class ObjectExtensions
{
    public static string ToJson(this object value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
