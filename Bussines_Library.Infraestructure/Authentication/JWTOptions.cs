namespace Bussines_Library.Infraestructure.Authentication
{
    public sealed class JWTOptions
    {
        public const string SectionName = "JWT";
        public bool Enabled { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; } = 60;

    }
}
