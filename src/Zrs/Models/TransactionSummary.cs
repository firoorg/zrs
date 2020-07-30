namespace Zrs.Models
{
    using NBitcoin;

    public class TransactionSummary
    {
        public uint256? Hash { get; set; }
        public uint Version { get; set; }
        public object? LockTime { get; set; }
        public ElysiumTransactionSummary? Elysium { get; set; }
        public int ZerocoinSpend { get; set; }
        public int SigmaSpend { get; set; }
    }
}
