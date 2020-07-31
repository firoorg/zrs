namespace Zrs.Models
{
    public sealed class TransactionsSummary
    {
        /// <summary>
        /// Number of all transactions.
        /// </summary>
        /// <example>
        /// 1
        /// </example>
        public int Count { get; set; }

        /// <summary>
        /// Number of all Elysium transactions.
        /// </summary>
        /// <example>
        /// 0
        /// </example>
        public int Elysium { get; set; }

        /// <summary>
        /// Number of all Zerocoin spend transactions.
        /// </summary>
        /// <example>
        /// 0
        /// </example>
        public int ZerocoinSpend { get; set; }

        /// <summary>
        /// Number of all Sigma spend transactions.
        /// </summary>
        /// <example>
        /// 0
        /// </example>
        public int SigmaSpend { get; set; }
    }
}
