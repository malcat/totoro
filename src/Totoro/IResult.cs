using System.Collections.Immutable;

namespace Totoro;

public interface IResult
{
    bool Succeeded { get; set; }

    string Error { get; set; }

    ImmutableArray<string>? Validations { get; set; }

    object? Metadata { get; set; }
}

public interface IResult<T> : IResult
{
    T Data { get; set; }
}
