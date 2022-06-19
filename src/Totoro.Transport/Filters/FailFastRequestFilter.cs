using System.Collections.Immutable;
using FluentValidation;
using MassTransit;

namespace Totoro.Transport.Filters;

public class FailFastRequestFilter<TRequest> : IFilter<ConsumeContext<TRequest>> where TRequest : class
{
    private readonly IEnumerable<IValidator> _validators;

    public FailFastRequestFilter(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task Send(ConsumeContext<TRequest> context, IPipe<ConsumeContext<TRequest>> next)
    {
        if (!_validators.Any())
        {
            await next.Send(context);
        }

        var validations = GetErrorMessages(context.Message);

        if (!validations.Any())
        {
            await next.Send(context);
        }

        await context.Publish<Result<object>>(new
        {
            Succeeded = false,
            Error = Errors.Validation,
            Validations = validations
        });
    }

    public void Probe(ProbeContext context)
    {
        // Intentionally left empty.
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
