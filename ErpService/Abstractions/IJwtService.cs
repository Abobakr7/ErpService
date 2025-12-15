using System.Security.Claims;


namespace ErpService.Abstractions
{
    public interface IJwtService
    {
        String GenerateToken(string Username);
        ClaimsPrincipal? ValidateToken(string token);
    }


}
