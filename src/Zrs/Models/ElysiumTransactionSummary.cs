namespace Zrs.Models
{
    using NBitcoin;

    /// <summary>
    /// Summary of the Elysium transaction.
    /// </summary>
    public sealed class ElysiumTransactionSummary
    {
        /// <summary>
        /// The sender address.
        /// </summary>
        /// <example>
        /// a95gwjFFBbJWTpAzNxFRbFGvJjZgntrgUj
        /// </example>
        public BitcoinAddress? Sender { get; set; }

        /// <summary>
        /// The receiver address.
        /// </summary>
        /// <example>
        /// a6RcniNC1E2UjxTrpqRu1CCaHTswTC4BWE
        /// </example>
        public BitcoinAddress? Receiver { get; set; }

        /// <summary>
        /// The transaction type.
        /// </summary>
        /// <example>
        /// 54
        /// </example>
        public int Type { get; set; }

        /// <summary>
        /// The transaction version.
        /// </summary>
        /// <example>
        /// 0
        /// </example>
        public int Version { get; set; }
    }
}
