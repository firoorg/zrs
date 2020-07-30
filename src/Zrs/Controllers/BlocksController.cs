namespace Zrs.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Zrs.Models;
    using Zrs.Options;
    using Zsharp.Bitcoin;
    using Zsharp.LightweightIndexer;

    [ApiController]
    [Route("blocks")]
    public sealed class BlocksController : ControllerBase
    {
        public const int DefaultDefaultListing = 10;
        public const int DefaultListingLimit = 100;

        readonly BlocksApiOptions options;
        readonly IBlockRepository blocks;

        public BlocksController(IOptionsSnapshot<BlocksApiOptions> options, IBlockRepository blocks)
        {
            this.options = options.Value;
            this.blocks = blocks;
        }

        public int DefaultListing => this.options.DefaultListing switch
        {
            null => DefaultDefaultListing,
            int v => v
        };

        public int ListingLimit => this.options.ListingLimit switch
        {
            null => DefaultListingLimit,
            int v when v < 0 => int.MaxValue,
            int v => v
        };

        [HttpGet]
        public async Task<IActionResult> ListBlockAsync(int? limit, CancellationToken cancellationToken = default)
        {
            // Validate parameters.
            if (limit != null && (limit < 0 || limit > this.ListingLimit))
            {
                this.ModelState.AddModelError(nameof(limit), "The value is not a valid limit number.");
                return this.BadRequest(this.ModelState);
            }

            limit ??= this.DefaultListing;

            // Load blocks and construct response.
            var (blocks, highest) = await this.blocks.GetLatestsBlocksAsync(limit.Value, cancellationToken);
            var response = blocks.Select((b, i) => new BlockSummary()
            {
                Height = highest - i,
                Hash = b.GetHash().ToString(),
                Time = b.Header.BlockTime.UtcDateTime,
                Tx = new TransactionsSummary()
                {
                    Count = b.Transactions.Count,
                    Elysium = b.Transactions.Count(t => t.GetElysiumTransaction() != null),
                },
            });

            return this.Ok(response);
        }
    }
}
