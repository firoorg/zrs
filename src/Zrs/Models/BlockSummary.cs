namespace Zrs.Models
{
    using NBitcoin;

    public sealed class BlockSummary : BlockInformation
    {
        public BlockSummary(int height, BlockHeader block, TransactionsSummary transaction)
            : base(height, block)
        {
            this.Transaction = transaction;
        }

        /// <summary>
        /// The summary of all transactions in this block.
        /// </summary>
        public TransactionsSummary Transaction { get; set; }
    }
}
