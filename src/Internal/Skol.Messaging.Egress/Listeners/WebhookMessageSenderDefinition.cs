using MassTransit;

namespace Skol.Messaging.Egress.Listeners;

public sealed class WebhookMessageSenderDefinition : ConsumerDefinition<WebhookMessageSender>
{
    public WebhookMessageSenderDefinition()
    {
        Endpoint(cfg => { cfg.Name = "egress-webhooks"; });
    }
}
