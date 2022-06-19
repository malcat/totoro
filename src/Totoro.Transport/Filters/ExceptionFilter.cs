using MassTransit;
using Microsoft.Extensions.Logging;

namespace Totoro.Transport.Filters;

public class ExceptionFilter<TRequest> : IFilter<ConsumeContext<TRequest>> where TRequest : class
{
    private readonly ILogger<ExceptionFilter<TRequest>> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter<TRequest>> logger)
    {
        _logger = logger;
    }

    public async Task Send(ConsumeContext<TRequest> context, IPipe<ConsumeContext<TRequest>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    public void Probe(ProbeContext context)
    {
        // Intentionally left empty.
    }

    #region Private Methods

    private async Task HandleExceptionAsync(ConsumeContext<TRequest> context, Exception ex)
    {
        var id = Guid.NewGuid().ToString();

        _logger.LogError(ex, "Transport Exception Handler Filter ({0})", id);

        await context.Publish<Result<object>>(Result.Fail(Errors.Woops, null, new
        {
            Trace = id
        }));
    }

    #endregion
}
