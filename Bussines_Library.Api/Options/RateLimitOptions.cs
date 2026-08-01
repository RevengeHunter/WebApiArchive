namespace Bussines_Library.Api.Options
{
    public sealed class RateLimitOptions
    {
        public const string SectionName = "RateLimitOptions";
        public int Limit { get; set; } = 100;
        public int WindowInSeconds { get; set; } = 60;
    }
}
