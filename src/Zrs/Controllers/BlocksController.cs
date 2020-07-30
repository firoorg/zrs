namespace Zrs.Controllers
{
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

        [HttpGet]
        [Route("{height}")]
        public async Task<IActionResult> GetBlockAsync(int height, CancellationToken cancellationToken = default)
        {
            // Load block.
            if (height < 0)
            {
                return this.NotFound();
            }

            var block = await this.blocks.GetBlockAsync(height, cancellationToken);

            if (block == null)
            {
                return this.NotFound();
            }

            // Construct response.
            var response = new BlockDetails()
            {
                Transactions = block.Transactions.Select(this.CreateTransactionSummary),
            };

            BlockInformation.Populate(response, block, height);

            return this.Ok(response);
        }

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
            var result = new BlockSummary()
            {
                Transaction = tx,
            };

            BlockInformation.Populate(result, block, height);

            return result;
        }

        TransactionSummary CreateTransactionSummary(Transaction tx)
        {
            // Common properties.
            TransactionSummary result = tx.Version switch
            {
                3 => new ExtendedTransactionSummary()
                {
                    Type = (int)tx.GetTransactionType(),
                },
                _ => new NormalTransactionSummary()
                {
                    CoinBase = tx.IsCoinBase,
                },
            };

            result.Hash = tx.GetHash();
            result.Version = tx.Version;
            result.LockTime = tx.LockTime.IsHeightLock ? (object)tx.LockTime.Height : tx.LockTime.Date;

            foreach (var i in tx.Inputs)
            {
                if (i.IsZerocoinSpend() || i.IsZerocoinRemint())
                {
                    result.ZerocoinSpend++;
                }

                if (i.IsSigmaSpend())
                {
                    result.SigmaSpend++;
                }
            }

            // Elysium.
            var elysium = tx.GetElysiumTransaction();

            if (elysium != null)
            {
                result.Elysium = new ElysiumTransactionSummary()
                {
                    Sender = elysium.Sender,
                    Receiver = elysium.Receiver,
                    Type = elysium.Id,
                    Version = elysium.Version,
                };
            }

            return result;
        }
    }
}
