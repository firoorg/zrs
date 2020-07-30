namespace Zrs.Models
{
    using System;
    using NBitcoin;

    public class BlockInformation
    {
        public int Height { get; set; }
        public uint256? Hash { get; set; }
        public DateTime Time { get; set; }
        public uint256? Target { get; set; }
        public double Difficulty { get; set; }
        public uint Nonce { get; set; }
        public uint256? PreviousBlock { get; set; }

        public static void Populate(BlockInformation target, Block source, int height)
        {
            var header = source.Header;

            target.Height = height;
            target.Hash = source.GetHash();
            target.Time = header.BlockTime.UtcDateTime;
            target.Target = header.Bits.ToUInt256();
            target.Difficulty = header.Bits.Difficulty;
            target.Nonce = header.Nonce;
            target.PreviousBlock = (height != 0) ? header.HashPrevBlock : null;
        }
    }
}
