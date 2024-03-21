using Ambition.Domain;

using MassTransit;

namespace Ambition.Infrastructure.Events;

internal class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _bus;

    public EventPublisher(IPublishEndpoint bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T @event, string topic) where T : class
    {
        await _bus.Publish(@event);
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
