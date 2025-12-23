namespace ErpService.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string secretKey { get; set; } = string.Empty;

        public string Issuer { get; set; } = "ErpService";

        public string Audience { get; set; } = "ErpServiceClients";

        public int ExpirationMinutes { get; set; } = 60;

    }
}
