namespace Zrs.Models
{
    using NBitcoin;

    /// <summary>
    /// Summary of the transaction in the normal form.
    /// </summary>
    public sealed class NormalTransactionSummary : TransactionSummary
    {
        public NormalTransactionSummary(Transaction tx)
            : base(tx)
        {
            this.CoinBase = tx.IsCoinBase;
        }

        /// <summary>
        /// Indicated this transaction is a coinbase transaction.
        /// </summary>
        /// <example>
        /// true
        /// </example>
        public bool CoinBase { get; set; }
    }
}
