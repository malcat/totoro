using System.Collections.Immutable;
using FluentValidation;
using MediatR;

namespace Totoro.Transport.Behaviors;

public class FailFastRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResult, new()
{
    private readonly IEnumerable<IValidator> _validators;

    public FailFastRequestBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken token,
        RequestHandlerDelegate<TResponse> next)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var validations = GetErrorMessages(request);

        if (!validations.Any())
        {
            return await next();
        }

        return new TResponse
        {
            Succeeded = false,
            Error = Errors.Validation,
            Validations = validations
        };
    }

    #region Private Methods

    private ImmutableArray<string> GetErrorMessages(TRequest request)
    {
        var context = new ValidationContext<TRequest>(request);

        return _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .Select(failure => failure.ErrorMessage)
            .ToImmutableArray();
    }

    #endregion
}
