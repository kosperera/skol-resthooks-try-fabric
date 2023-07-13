using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skol.Messaging.Contracts;
using Skol.Messaging.Ingress.Domain;
using Skol.Messaging.Ingress.Internals.Http;

namespace Skol.Messaging.Ingress.Endpoints.Send
{
    [Route("v1/messaging/publish")]
    [ApiController]
    public sealed partial class PublishEndpoint : ControllerBase
    {
        readonly IPublishEndpoint _publisher;
        readonly ILogger<PublishEndpoint> _logger;

        public PublishEndpoint(IPublishEndpoint publisher, ILogger<PublishEndpoint> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        [HttpPost]
        public async ValueTask<IActionResult> ExecuteAsync(
            [FromBody] object payload,
            CancellationToken cancellationToken = default)
        {
            Request.Headers.TryGetValue(KnownHeaderNames.RequestId, out var requestId);
            Trace.Assert(
                condition: payload is { },
                message: $"[Resthooks] Ingress notification ingress attempt failed. (RequestId={requestId})");

            if (payload is null) { return BadRequest(); }

            await _publisher.Publish<MessageIngested>(new
            {
                OccurredAsOf = Request.GetTypedHeaders().Date ?? DateTimeOffset.UtcNow,
                EventKind = Request.Headers[KnownHeaderNames.EventKind].AsString(),
                Environment = Request.Headers[KnownHeaderNames.Environment].AsString(),

                Headers = Request.Headers.WithoutPoisoned(),
                Content = payload,
            },
            callback: (PublishContext<MessageIngested> ctx) =>
            {
                ctx.SetRoutingKey(ctx.Message.EventKind);
            },
            cancellationToken);

            return Accepted();
        }
    }
}
