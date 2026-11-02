using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OnlineAcadimy.Services
{
    public class AuthService : IAuthService
    {
        private readonly OnlineAcademyDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(OnlineAcademyDbContext context,IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            var email = dto.Email.Trim();

            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return false;
            }

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                Role = "Student"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user,dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<User?> LoginAsync(LoginDTO dto)
        {
            string email = dto.Email.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash,dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }
    }
}