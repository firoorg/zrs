namespace Zrs.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using NBitcoin;
    using Zrs.Models;
    using Zrs.Options;
    using Zsharp.Bitcoin;
    using IBlockRepository = Zsharp.LightweightIndexer.IBlockRepository;

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

        /// <summary>
        /// Retrieves a specific block by height.
        /// </summary>
        /// <param name="height">
        /// The block height to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        /// </param>
        /// <response code="404">The specified height is not valid.</response>
        [HttpGet("{height:int}")]
        [ProducesResponseType(typeof(BlockDetails), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBlockAsync(int height, CancellationToken cancellationToken = default)
        {
            if (height < 0)
            {
                return this.NotFound();
            }

            var block = await this.blocks.GetBlockAsync(height, cancellationToken);

            if (block == null)
            {
                return this.NotFound();
            }

            return this.Ok(new BlockDetails(height, block));
        }

        /// <summary>
        /// Retrieves a specific block by hash.
        /// </summary>
        /// <param name="hash">
        /// The block hash to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        /// </param>
        /// <response code="404">The specified hash is not valid.</response>
        [HttpGet("{hash:uint256}")]
        [ProducesResponseType(typeof(BlockDetails), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBlockAsync(uint256 hash, CancellationToken cancellationToken = default)
        {
            var (block, height) = await this.blocks.GetBlockAsync(hash, cancellationToken);

            if (block == null)
            {
                return this.NotFound();
            }

            return this.Ok(new BlockDetails(height, block));
        }

        /// <summary>
        /// Retrieves latest blocks.
        /// </summary>
        /// <param name="limit">
        /// Maximum number of latest blocks to return.
        /// </param>
        /// <param name="cancellationToken">
        /// </param>
        /// <response code="400"><paramref name="limit"/> is not a valid value.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlockSummary>), 200)]
        [ProducesResponseType(400)]
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
            var response = blocks.Select((b, i) => this.CreateBlockSummary(highest - i, b));

            return this.Ok(response);
        }

        BlockSummary CreateBlockSummary(int height, Block block)
        {
            // Transaction summary.
            var tx = new TransactionsSummary()
            {
                Count = block.Transactions.Count,
            };

            foreach (var t in block.Transactions)
            {
                if (t.GetElysiumTransaction() != null)
                {
                    tx.Elysium++;
                }

                if (t.IsZerocoinSpend() || t.IsZerocoinRemint())
                {
                    tx.ZerocoinSpend++;
                }

                if (t.IsSigmaSpend())
                {
                    tx.SigmaSpend++;
                }
            }

            // Block summary.
            return new BlockSummary(height, block.Header, tx);
        }
    }
}
