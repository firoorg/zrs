namespace Zrs.Models
{
    using System.Collections;
    using System.Linq;
    using NBitcoin;
    using Zrs.Swagger;

    public sealed class BlockDetails : BlockInformation
    {
        public BlockDetails(int height, Block block)
            : base(height, block.Header)
        {
            this.Transactions = block.Transactions.Select(CreateTransactionSummary);
        }

        /// <summary>
        /// The summary of all transaction in the block.
        /// </summary>
        [Type(typeof(NormalTransactionSummary), typeof(ExtendedTransactionSummary))]
        public IEnumerable Transactions { get; set; }

        static TransactionSummary CreateTransactionSummary(Transaction tx)
        {
            TransactionSummary result = tx.Version switch
            {
                3 => new ExtendedTransactionSummary(tx),
                _ => new NormalTransactionSummary(tx),
            };

            return result;
        }
    }
}
