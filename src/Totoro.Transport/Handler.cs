using MassTransit;
using MediatR;

namespace Totoro.Transport;

public abstract class Handler<TRequest, TResponse> :
    IRequestHandler<TRequest, TResponse>,
    IConsumer<TRequest>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    protected abstract Task<TResponse> HandleAsync(TRequest request);

    // From: MediatR.
    public async Task<TResponse> Handle(TRequest request, CancellationToken token)
    {
        return await HandleAsync(request);
    }

    // From: MassTransit.
    public async Task Consume(ConsumeContext<TRequest> context)
    {
        var request = context.Message;
        var response = await HandleAsync(request);

        await context.RespondAsync(response);
    }
}
