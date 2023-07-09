using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.Activate
{
    [Route("v1/subscription")]
    [ApiController]
    public sealed partial class ActivateEndpoint : ControllerBase
    {
        public sealed class ActivateSubscription
        {
            [property: JsonPropertyName("activation_code")]
            public string Code { get; set; }
        }

        readonly IIntentsDb _db;
        readonly ILogger<ActivateEndpoint> _logger;

        public ActivateEndpoint(IIntentsDb db, ILogger<ActivateEndpoint> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPatch("id/{id:guid}/activate")]
        public async ValueTask<IActionResult> ExecuteAsync(
            [FromRoute] Guid id,
            [FromBody] ActivateSubscription content,
            CancellationToken cancellationToken = default)
        {

            if (content is null) { return BadRequest(); }

            Subscription entity = await _db.Subscriptions.Disabled()
                                                         .WithId(id)
                                                         .AsTracking()
                                                         .SingleOrDefaultAsync(cancellationToken);

            var routeValues = new { id };

            if (entity is null) { return NotFound(routeValues); }
            else if (entity.ActivationCode != content.Code) { return Unauthorized(routeValues); }

            entity.Enabled = true;

            _db.Subscriptions.Update(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
