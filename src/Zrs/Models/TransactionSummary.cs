namespace Zrs.Models
{
    using System;
    using NBitcoin;
    using Zrs.Swagger;
    using Zsharp.Bitcoin;

    public class TransactionSummary
    {
        public TransactionSummary(Transaction tx)
        {
            var elysium = tx.GetElysiumTransaction();

            this.Hash = tx.GetHash();
            this.Version = tx.Version;
            this.LockTime = tx.LockTime.IsHeightLock ? (object)tx.LockTime.Height : tx.LockTime.Date.UtcDateTime;

            if (elysium != null)
            {
                this.Elysium = new ElysiumTransactionSummary()
                {
                    Sender = elysium.Sender,
                    Receiver = elysium.Receiver,
                    Type = elysium.Id,
                    Version = elysium.Version,
                };
            }

            foreach (var i in tx.Inputs)
            {
                if (i.IsZerocoinSpend() || i.IsZerocoinRemint())
                {
                    this.ZerocoinSpend++;
                }

                if (i.IsSigmaSpend())
                {
                    this.SigmaSpend++;
                }
            }
        }

        /// <summary>
        /// Hash of this transaction.
        /// </summary>
        /// <example>
        /// e6055e8523f7bdc5f8aef36aa5c3ab0f012455b59fce6ee601c606c5c9040146
        /// </example>
        public uint256 Hash { get; set; }

        /// <summary>
        /// Version of this transaction.
        /// </summary>
        /// <example>
        /// 1
        /// </example>
        public uint Version { get; set; }

        /// <summary>
        /// Locktime of this transaction.
        /// </summary>
        [Type(typeof(int), typeof(DateTime))]
        [Example(200000, "2020-08-31T10:34:20.052Z")]
        public object LockTime { get; set; }

        /// <summary>
        /// Contains summary for Elysium transaction.
        /// </summary>
        public ElysiumTransactionSummary? Elysium { get; set; }

        /// <summary>
        /// Number of Zerocoin spend input.
        /// </summary>
        /// <example>
        /// 0
        /// </example>
        public int ZerocoinSpend { get; set; }

        /// <summary>
        /// Number of Sigma spend input.
        /// </summary>
        /// <example>
        /// 0
        /// </example>
        public int SigmaSpend { get; set; }
    }
}
