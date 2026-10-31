using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAcadimy.Constants;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Services;

namespace OnlineAcadimy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }
        [HttpPut("{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(int userId,[FromBody] UpdateUserRoleDTO dto)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var updated = await _userService.UpdateUserRoleAsync(userId, dto);

                if (!updated)
                {
                    return NotFound("User not found.");
                }

                return Ok("User role updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}