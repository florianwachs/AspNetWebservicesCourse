using System.Collections.Concurrent;
using System.Threading.Channels;

namespace TechConf.Grpc.Server.Services;

public sealed class EventStreamBroadcaster
{
    private readonly ConcurrentDictionary<Guid, Channel<EventMessage>> _subscribers = new();

    public ChannelReader<EventMessage> Subscribe(out Guid subscriptionId)
    {
        subscriptionId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<EventMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _subscribers[subscriptionId] = channel;
        return channel.Reader;
    }

    public void Publish(EventMessage message)
    {
        foreach (var (subscriptionId, channel) in _subscribers)
        {
            if (!channel.Writer.TryWrite(message))
            {
                Unsubscribe(subscriptionId);
            }
        }
    }

    public void Unsubscribe(Guid subscriptionId)
    {
        if (_subscribers.TryRemove(subscriptionId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }
}
