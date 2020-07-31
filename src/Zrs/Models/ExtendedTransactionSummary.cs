namespace Zrs.Models
{
    using NBitcoin;
    using Zsharp.Bitcoin;
    using Zsharp.Zcoin;

    public sealed class ExtendedTransactionSummary : TransactionSummary
    {
        public ExtendedTransactionSummary(Transaction tx)
            : base(tx)
        {
            this.Type = tx.GetTransactionType();
        }

        /// <summary>
        /// Type of this transaction.
        /// </summary>
        /// <example>
        /// 5
        /// </example>
        public TransactionType Type { get; set; }
    }
}
