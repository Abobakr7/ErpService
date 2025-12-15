using ErpService.Configuration;
using Microsoft.Extensions.Options;

namespace ErpService.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiKeySettings _apiKeySettings;
        private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

        public ApiKeyAuthenticationMiddleware(
            RequestDelegate next,
            IOptions<ApiKeySettings> apiKeySettings,
            ILogger<ApiKeyAuthenticationMiddleware> logger)
        {
            _next = next;
            _apiKeySettings = apiKeySettings.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsPublicPath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // API key from header
            if (!context.Request.Headers.TryGetValue(_apiKeySettings.HeaderName, out var extractedApiKey))
            {
                _logger.LogWarning("API Key missing in request from {IpAddress}",
                    context.Connection.RemoteIpAddress);

                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "API Key is missing",
                    message = $"Please provide API key in '{_apiKeySettings.HeaderName}' header"
                });
                return;
            }

            // Validate API key
            if (!_apiKeySettings.ValidApiKeys.Contains(extractedApiKey.ToString()))
            {
                _logger.LogWarning("Invalid API Key attempted from {IpAddress}",
                    context.Connection.RemoteIpAddress);

                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Invalid API Key",
                    message = "The provided API key is not valid"
                });
                return;
            }

            _logger.LogDebug("API Key validated successfully");

            await _next(context);
        }

        private bool IsPublicPath(PathString path)
        {
            var publicPaths = new[]
            {
                "/health",
                "/swagger",
                "/api-docs"
            };

            return publicPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
        }
    }
    public static class ApiKeyAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        }
    }
}