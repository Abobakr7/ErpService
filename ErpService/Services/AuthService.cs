using ErpService.Abstractions;
using ErpService.Dtos;  


namespace ErpService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IJwtService jwtService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Authentication failed: request is null");
                return null;
            }

            if (!await ValidateTicketAsync(request.Username, request.Password))
            {
                _logger.LogWarning("Authentication failed for user: {Username}", request.Username);
                return null;
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(request.Username);
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60);

            _logger.LogInformation("User authenticated successfully: {Username}", request.Username);

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                TokenType = "Bearer",
                Username = request.Username,
            };
        }

        private async Task<bool> ValidateTicketAsync(string username, string password)
        {
            // Validate from configuration
            var validUsers = _configuration.GetSection("AuthUsers").Get<Dictionary<string, string>>();

            if (validUsers == null || !validUsers.ContainsKey(username))
            {
                return false;
            }

            var isValid = validUsers[username] == password;

            await Task.CompletedTask;

            return isValid;
        }
    }
}

