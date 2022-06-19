using System.Collections.Immutable;

namespace Totoro;

/// <summary>
/// It provides a protocol to standardize communication between local and remote services.
/// </summary>
public sealed class Result : IResult
{
    /// <summary>
    /// Whether the process was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Error that occurred during the process.
    /// </summary>
    public string Error { get; set; } = null!;

    /// <summary>
    /// If there were any validation errors during the process.
    /// </summary>
    public ImmutableArray<string>? Validations { get; set; }

    /// <summary>
    /// Any information related to the Error.
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// Create a new successful result.
    /// </summary>
    /// <returns>A result.</returns>
    public static Result Success()
    {
        return new()
        {
            Succeeded = true
        };
    }

    /// <summary>
    /// Create a new unsuccessful result.
    /// </summary>
    /// <param name="error">The resulting error.</param>
    /// <param name="validations">The validation errors.</param>
    /// <returns>A result.</returns>
    public static Result Fail(string error, ICollection<string>? validations = null)
    {
        return new()
        {
            Succeeded = false,
            Error = error,
            Validations = validations?.ToImmutableArray()
        };
    }

    /// <summary>
    /// Create a new unsuccessful result.
    /// </summary>
    /// <param name="error">The resulting error.</param>
    /// <param name="validations">The validation errors.</param>
    /// <param name="metadata">Any information related to the Error.</param>
    /// <returns>A result.</returns>
    public static Result Fail(string error, ICollection<string>? validations, object? metadata)
    {
        return new()
        {
            Succeeded = false,
            Error = error,
            Validations = validations?.ToImmutableArray(),
            Metadata = metadata
        };
    }

    /// <summary>
    /// Create a new unsuccessful result from a source.
    /// </summary>
    /// <param name="source">The source result.</param>
    /// <returns>A result.</returns>
    public static Result Fail(Result source)
    {
        return Fail(source.Error, source.Validations, source.Metadata);
    }

    /// <summary>
    /// Create a new unsuccessful result from a source.
    /// </summary>
    /// <param name="source">The source result.</param>
    /// <returns>A result.</returns>
    public static Result Fail(Result<object> source)
    {
        return Fail(source.Error, source.Validations, source.Metadata);
    }
}

/// <summary>
/// It provides a protocol to standardize communication between local and remote services.
/// </summary>
/// <typeparam name="T">The resulting data type.</typeparam>
public sealed class Result<T> : IResult<T>
{
    /// <summary>
    /// Whether the process was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Data that was produced during the process.
    /// </summary>
    public T Data { get; set; } = default!;

    /// <summary>
    /// Error that occurred during the process.
    /// </summary>
    public string Error { get; set; } = null!;

    /// <summary>
    /// If there were any validation errors during the process.
    /// </summary>
    public ImmutableArray<string>? Validations { get; set; }

    /// <summary>
    /// Any information related to the Error.
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    /// Create a new successful result.
    /// </summary>
    /// <param name="data">The resulting data.</param>
    /// <returns>A result.</returns>
    public static Result<T> Success(T data)
    {
        return new()
        {
            Succeeded = true,
            Data = data
        };
    }

    /// <summary>
    /// Create a new unsuccessful result.
    /// </summary>
    /// <param name="error">The resulting error.</param>
    /// <param name="validations">The validation errors.</param>
    /// <returns>A result.</returns>
    public static Result<T> Fail(string error, ICollection<string>? validations = null)
    {
        return new()
        {
            Succeeded = false,
            Error = error,
            Validations = validations?.ToImmutableArray()
        };
    }

    /// <summary>
    /// Create a new unsuccessful result.
    /// </summary>
    /// <param name="error">The resulting error.</param>
    /// <param name="validations">The validation errors.</param>
    /// <param name="metadata">Any information related to the Error.</param>
    /// <returns>A result.</returns>
    public static Result<T> Fail(string error, ICollection<string>? validations, object? metadata)
    {
        return new()
        {
            Succeeded = false,
            Error = error,
            Validations = validations?.ToImmutableArray(),
            Metadata = metadata
        };
    }

    /// <summary>
    /// Create a new unsuccessful result from a source.
    /// </summary>
    /// <param name="source">The source result.</param>
    /// <returns>A result.</returns>
    public static Result<T> Fail(Result source)
    {
        return Fail(source.Error, source.Validations, source.Metadata);
    }

    /// <summary>
    /// Create a new unsuccessful result from a source.
    /// </summary>
    /// <param name="source">The source result.</param>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <returns>A result.</returns>
    public static Result<T> Fail<TSource>(Result<TSource> source)
    {
        return Fail(source.Error, source.Validations, source.Metadata);
    }
}
