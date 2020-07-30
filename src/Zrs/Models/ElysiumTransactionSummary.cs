namespace Zrs.Models
{
    using NBitcoin;

    public sealed class ElysiumTransactionSummary
    {
        public BitcoinAddress? Sender { get; set; }
        public BitcoinAddress? Receiver { get; set; }
        public int Type { get; set; }
        public int Version { get; set; }
    }
}
