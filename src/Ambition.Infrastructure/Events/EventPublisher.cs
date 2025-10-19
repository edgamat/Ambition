using Ambition.Domain;

using Edgamat.Messaging;

namespace Ambition.Infrastructure.Events;

internal class EventPublisher : IEventPublisher
{
    private readonly IPublisher _publisher;

    public EventPublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync<T>(T @event, string topic) where T : class
    {
        await _publisher.PublishAsync(topic, @event);
    }
}

//internal class EventPublisher : IEventPublisher
//{
//    private readonly ISendEndpointProvider _sendEndpointProvider;

//    public EventPublisher(ISendEndpointProvider sendEndpointProvider)
//    {
//        _sendEndpointProvider = sendEndpointProvider;
//    }

//    public async Task PublishAsync<T>(T @event, string topic) where T : class
//    {
//        var thePath = new Uri($"exchange:{topic}");

//        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(thePath);

//        await sendEndpoint.Send(@event);
//    }
//}
