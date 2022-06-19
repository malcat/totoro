using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MassTransit;

namespace Totoro.Transport.Internal;

internal class CustomEndpointNameFormatter : IEndpointNameFormatter
{
    private readonly string _prefix;

    private readonly IEndpointNameFormatter _formatter;

    public CustomEndpointNameFormatter(string prefix)
    {
        _prefix = $"{FormatName(prefix)}_";
        _formatter = new DefaultEndpointNameFormatter(_prefix, false);
    }

    public string Separator => string.Empty;

    public string TemporaryEndpoint(string tag)
    {
        return _formatter.TemporaryEndpoint(tag);
    }

    public string Consumer<T>() where T : class, IConsumer
    {
        var name = _formatter.Consumer<T>().Replace(_prefix, string.Empty);
        var hash = GetTypeHash<T>();

        name = RemoveSufix(name, "Handler");
        name = RemoveSufix(name, "Request");

        return $"{_prefix}{hash}_{name}";
    }

    public string Message<T>() where T : class
    {
        return _formatter.Message<T>();
    }

    public string Saga<T>() where T : class, ISaga
    {
        return _formatter.Saga<T>();
    }

    public string ExecuteActivity<T, TArguments>() where T : class, IExecuteActivity<TArguments> where TArguments : class
    {
        return _formatter.ExecuteActivity<T, TArguments>();
    }

    public string CompensateActivity<T, TLog>() where T : class, ICompensateActivity<TLog> where TLog : class
    {
        return _formatter.CompensateActivity<T, TLog>();
    }

    public string SanitizeName(string name)
    {
        return _formatter.SanitizeName(name);
    }

    #region Private Methods

    private string GetTypeHash<T>()
    {
        using var provider = SHA256.Create();
        var target = GetTypeNamespace<T>();
        var hash = provider.ComputeHash(Encoding.UTF8.GetBytes(target));
        var value = BitConverter.ToUInt32(hash, 0) % 1000000;

        return value.ToString();
    }

    private string GetTypeNamespace<T>()
    {
        var type = typeof(T);
        var name = type.Namespace ?? type.Name;
        var parts = name.Split('.').ToList();
        var count = parts.Count;

        // In some cases commands are grouped into folders by category. If the namespace
        // is structured this way, we remove the last part, to get just the common
        // namespace among other commands at the same directory level.
        // Example: Commands/CreateUser/CreateUserRequestHandler.cs
        if (count > 1 && type.Name.StartsWith(parts.Last()))
        {
            parts.RemoveAt(count - 1);
        }

        return string.Join('.', parts);
    }

    private string FormatName(string prefix)
    {
        return Regex.Replace(prefix, @"[^0-9a-zA-Z]", string.Empty);
    }

    private string RemoveSufix(string value, string sufix)
    {
        return value.EndsWith(sufix) ? value.Replace(sufix, string.Empty) : value;
    }

    #endregion
}
