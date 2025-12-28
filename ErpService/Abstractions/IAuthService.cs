using ErpService.Dtos;

namespace ErpService.Abstractions
{
    public interface IAuthService
    {
        LoginResponse? Authenticate(LoginRequest request);
    }
}
