using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO dto);

        Task<User?> LoginAsync(LoginDTO dto);
    }
}