using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skol.Resthooks.Subs.Domain.Models;
using Skol.Resthooks.Subs.Internals.Storage;

namespace Skol.Resthooks.Subs.Endpoints.Topics.List
{
    [ApiController]
    [Route("v1/topics")]
    public sealed partial class TopicsEndpoint : ControllerBase
    {
        readonly IIntentsDb _db;
        readonly ILogger<TopicsEndpoint> _logger;

        public TopicsEndpoint(IIntentsDb db, ILogger<TopicsEndpoint> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async ValueTask<IActionResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Topic[] result = await _db.Topics.ToArrayAsync(cancellationToken);

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
