namespace Zrs.Models
{
    using System;

    public sealed class BlockSummary
    {
        public int Height { get; set; }
        public string? Hash { get; set; }
        public DateTime Time { get; set; }
        public TransactionsSummary? Tx { get; set; }
    }
}
