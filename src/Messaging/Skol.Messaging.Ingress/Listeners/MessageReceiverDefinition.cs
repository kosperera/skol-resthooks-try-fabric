using MassTransit;
using Skol.Messaging.Ingress.Listeners;

namespace Skol.Messaging.Ingress.Internals.Listeners;

public sealed class MessageReceiverDefinition : ConsumerDefinition<MessageReceiver>
{
    public MessageReceiverDefinition()
    {
        Endpoint(cfg => { cfg.Name = "digest-ingress"; });
    }
}
