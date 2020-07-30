namespace Zrs.Models
{
    public sealed class BlockSummary : BlockInformation
    {
        public TransactionsSummary? Transaction { get; set; }
    }
}
