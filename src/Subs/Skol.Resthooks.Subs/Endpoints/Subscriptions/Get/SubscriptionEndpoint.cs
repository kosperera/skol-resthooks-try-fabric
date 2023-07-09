using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.Get
{
    [Route("v1/subscription")]
    [ApiController]
    public sealed partial class SubscriptionEndpoint : ControllerBase
    {
        readonly IIntentsDb _db;
        readonly ILogger<SubscriptionEndpoint> _logger;

        public SubscriptionEndpoint(IIntentsDb db, ILogger<SubscriptionEndpoint> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("id/{id}")]
        public async ValueTask<IActionResult> ExecuteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            Subscription entity = await _db.Subscriptions.WithId(id)
                                                         .SingleOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return NotFound(new { id });
            }
            else
            {
                return Ok(entity);
            }
        }
    }
}
