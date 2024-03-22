namespace Ambition.Accounting.Events;

public interface IEventHandler<T>
{
    Task HandleAsync(T @event);
}
