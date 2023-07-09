using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Skol.Messaging.Contracts;
using Skol.Messaging.Egress.Internals.Http;

namespace Skol.Messaging.Egress.Listeners
{
    public sealed partial class WebhookMessageSender : IConsumer<MessageDigested>
    {
        readonly WebhookHttpClient _proxy;
        readonly ILogger<WebhookMessageSender> _logger;

        public WebhookMessageSender(WebhookHttpClient proxy, ILogger<WebhookMessageSender> logger)
        {
            _proxy = proxy;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MessageDigested> context)
        {
            MessageDigested msg = context.Message;

            await _proxy.PostJsonAsync(
                msg.NotificationUrl,
                msg.Content,
                (req) =>
                {
                    foreach (var header in msg.Headers)
                    {
                        req.Headers.Add(header.Key, header.Value);
                    }
                    req.Headers.Date = msg.OccurredAsOf;
                    req.Headers.Add(KnownHeaderNames.CorrelationId, $"{context.CorrelationId}");
                });
        }
    }
}
