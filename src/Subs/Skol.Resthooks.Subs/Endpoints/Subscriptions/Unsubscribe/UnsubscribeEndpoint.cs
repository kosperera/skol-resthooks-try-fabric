using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.Unsubscribe
{
    [Route("v1/subscription")]
    [ApiController]
    public sealed partial class UnsubscribeEndpoint : ControllerBase
    {
        readonly IIntentsDb _db;
        readonly ILogger<UnsubscribeEndpoint> _logger;

        public UnsubscribeEndpoint(IIntentsDb db, ILogger<UnsubscribeEndpoint> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpDelete("id/{id:guid}")]
        public async ValueTask<IActionResult> ExecuteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            Subscription entity = await _db.Subscriptions.WithId(id)
                                                         .AsTracking()
                                                         .SingleOrDefaultAsync(cancellationToken);

            if (entity is null) { return NotFound(new { id }); }

            entity.Archived = true;

            _db.Subscriptions.Update(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
