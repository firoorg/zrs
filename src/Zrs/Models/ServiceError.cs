namespace Zrs.Models
{
    public sealed class ServiceError
    {
        public string? Service { get; set; }
        public string? Error { get; set; }
        public string? Details { get; set; }
    }
}
