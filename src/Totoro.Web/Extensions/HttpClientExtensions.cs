using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace Totoro.Web.Extensions;

public static class HttpClientExtensions
{
    #region Configuration

    public static HttpClient SetBaseAddress(this HttpClient http, string uri)
    {
        http.BaseAddress = new Uri(uri);

        return http;
    }

    #endregion

    #region HTTP Methods

    public static async Task<(TData, TError)> GetFromJsonAsync<TData, TError>(this HttpClient http, string uri)
    {
        var response = await http.GetAsync(uri);

        return await response.DeserializeJsonAsync<TData, TError>();
    }

    public static async Task<(TData, TError)> PostAsJsonAsync<TData, TError>(
        this HttpClient http,
        string uri,
        object data)
    {
        var response = await http.PostAsJsonAsync(uri, data);

        return await response.DeserializeJsonAsync<TData, TError>();
    }

    public static async Task<(TData, TError)> PutAsJsonAsync<TData, TError>(
        this HttpClient http,
        string uri,
        object data)
    {
        var response = await http.PutAsJsonAsync(uri, data);

        return await response.DeserializeJsonAsync<TData, TError>();
    }

    public static async Task<TError> DeleteFromJsonAsync<TError>(this HttpClient http, string uri)
    {
        var response = await http.DeleteAsync(uri);

        return await response.DeserializeJsonAsync<TError>();
    }

    #endregion

    #region Private Methods

    private static void EnsureJsonMediaType(this HttpResponseMessage response)
    {
        var type = response.Content.Headers.ContentType;

        if (type is not { MediaType: MediaTypeNames.Application.Json })
        {
            throw new InvalidOperationException("Media type must be JSON only");
        }
    }

    private static async Task<T> DeserializeJsonAsync<T>(this HttpResponseMessage response)
    {
        response.EnsureJsonMediaType();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json)!;
    }

    private static async Task<(TData, TError)> DeserializeJsonAsync<TData, TError>(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return (default, await response.DeserializeJsonAsync<TError>())!;
        }

        return (await response.DeserializeJsonAsync<TData>(), default)!;
    }

    #endregion
}
