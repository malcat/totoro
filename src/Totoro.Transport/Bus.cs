using MassTransit;
using MediatR;

namespace Totoro.Transport;

public class Bus : IBus
{
    private readonly IMediator _mediator;

    private readonly MassTransit.IBus _bus;

    public Bus(IMediator mediator, MassTransit.IBus bus)
    {
        _mediator = mediator;
        _bus = bus;
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, bool remote = false, CancellationToken token = default) where TRequest : class, IRequest<TResponse> where TResponse : class
    {
        if (remote)
        {
            return await RemoteSendAsync<TRequest, TResponse>(request, token);
        }

        return await LocalSendAsync<TRequest, TResponse>(request, token);
    }

    #region Private Methods

    // From: MassTransit.
    private async Task<TResponse> RemoteSendAsync<TRequest, TResponse>(TRequest request, CancellationToken token) where TRequest : class, IRequest<TResponse> where TResponse : class
    {
        var client = _bus.CreateRequestClient<TRequest>();
        var response = await client.GetResponse<TResponse>(request, token);

        return response.Message;
    }

    // From: MediatR.
    private async Task<TResponse> LocalSendAsync<TRequest, TResponse>(TRequest request, CancellationToken token) where TRequest : class, IRequest<TResponse> where TResponse : class
    {
        return await _mediator.Send(request, token);
    }

    #endregion
}
