namespace Ambition.Domain;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, string topic) where T : class;
}