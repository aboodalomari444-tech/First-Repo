using Microsoft.EntityFrameworkCore;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;
using System;
using System.Linq;

namespace OnlineAcadimy.Services
{
    public class UserService : IUserService
    {
        private readonly OnlineAcademyDbContext _context;

        public UserService(OnlineAcademyDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<bool> UpdateUserRoleAsync(int userId, UpdateUserRoleDTO dto)
        {
            var allowedRoles = new[] { "Admin", "Instructor", "Student" };

            if (!allowedRoles.Contains(dto.Role))
            {
                throw new ArgumentException("Invalid role.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return false;
            }

            user.Role = dto.Role;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}