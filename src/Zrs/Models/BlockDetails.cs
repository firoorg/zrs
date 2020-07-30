namespace Zrs.Models
{
    using System.Collections;

    public sealed class BlockDetails : BlockInformation
    {
        public IEnumerable? Transactions { get; set; }
    }
}
