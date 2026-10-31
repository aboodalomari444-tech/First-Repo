using OnlineAcadimy.DTOs;

namespace OnlineAcadimy.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<bool> UpdateUserRoleAsync(int userId, UpdateUserRoleDTO dto);
     
    }
}