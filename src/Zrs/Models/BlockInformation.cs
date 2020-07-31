namespace Zrs.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using NBitcoin;

    public class BlockInformation
    {
        public BlockInformation(int height, BlockHeader block)
        {
            this.Height = height;
            this.Hash = block.GetHash();
            this.Time = block.BlockTime.UtcDateTime;
            this.Target = block.Bits.ToUInt256();
            this.Difficulty = block.Bits.Difficulty;
            this.Nonce = block.Nonce;
            this.PreviousBlock = (height != 0) ? block.HashPrevBlock : null;
        }

        /// <summary>
        /// Height of this block.
        /// </summary>
        /// <example>
        /// 1
        /// </example>
        public int Height { get; set; }

        /// <summary>
        /// Hash of this block.
        /// </summary>
        /// <example>
        /// 626570cc8f1015c7d06e21d90ecd1cdb37012632acfc69e3c375a01b65cc44a7
        /// </example>
        public uint256 Hash { get; set; }

        /// <summary>
        /// Date and time when this block was created.
        /// </summary>
        /// <example>
        /// 2020-07-31T10:34:20.052Z
        /// </example>
        public DateTime Time { get; set; }

        /// <summary>
        /// Target threshold of this block, in a 256-bit form.
        /// </summary>
        /// <example>
        /// 7fffff0000000000000000000000000000000000000000000000000000000000
        /// </example>
        public uint256 Target { get; set; }

        /// <summary>
        /// Difficulty of this block.
        /// </summary>
        /// <example>
        /// 4.65e-10
        /// </example>
        public double Difficulty { get; set; }

        /// <summary>
        /// Nonce of this block.
        /// </summary>
        /// <example>
        /// 2
        /// </example>
        public uint Nonce { get; set; }

        /// <summary>
        /// Hash of the previous block, <c>null</c> if this block is a genesis block.
        /// </summary>
        /// <example>
        /// a42b98f04cc2916e8adfb5d9db8a2227c4629bc205748ed2f33180b636ee885b
        /// </example>
        public uint256? PreviousBlock { get; set; }
    }
}
