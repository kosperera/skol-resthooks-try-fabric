using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;
using static System.Environment;

namespace Skol.Resthooks.Subs.Endpoints.Subscriptions.List
{
    [Route("v1/subscriptions")]
    [ApiController]
    public sealed partial class SubscriptionsEndpoint : ControllerBase
    {
        readonly IIntentsDb _db;
        readonly ILogger<SubscriptionsEndpoint> _logger;

        public SubscriptionsEndpoint(IIntentsDb db, ILogger<SubscriptionsEndpoint> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async ValueTask<IActionResult> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            Subscription[] result = await _db.Subscriptions.WithEnvironment(GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                                                           .ToArrayAsync(cancellationToken);

            if (result is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
