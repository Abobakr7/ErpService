namespace ErpService.Configuration
{
    public class ApiKeySettings
    {
        public const string SectionName = "ApiKeySettings";

        public List<string> ValidApiKeys { get; set; } = new();

        public string HeaderName { get; set; } = "X-API-Key";
    }
}
