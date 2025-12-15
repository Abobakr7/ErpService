using ErpService.Dtos;
using Microsoft.AspNetCore.Identity.Data;

namespace ErpService.Abstractions
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(ErpService.Dtos.LoginRequest request);
    }
}
