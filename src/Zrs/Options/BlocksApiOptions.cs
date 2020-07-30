namespace Zrs.Options
{
    using System.ComponentModel.DataAnnotations;

    public sealed class BlocksApiOptions
    {
        [Range(0, int.MaxValue)]
        public int? DefaultListing { get; set; }

        public int? ListingLimit { get; set; }
    }
}
