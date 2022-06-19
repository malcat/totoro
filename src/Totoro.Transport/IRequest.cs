namespace Totoro.Transport;

public interface IRequest<out TResponse> : IMessage, MediatR.IRequest<TResponse> where TResponse : class
{
    // Intentionally left empty.
}
