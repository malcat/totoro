namespace Totoro.Transport;

public interface IBus
{
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, bool remote = false, CancellationToken token = default) where TRequest : class, IRequest<TResponse> where TResponse : class;
}
