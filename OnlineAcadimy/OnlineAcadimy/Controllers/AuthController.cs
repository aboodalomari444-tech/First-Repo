using Microsoft.AspNetCore.Mvc;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Services;

namespace OnlineAcadimy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService,IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            bool result = await _authService.RegisterAsync(dto);

            if (!result)
            {
                return Conflict("Email already exists.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       
        public async Task<IActionResult>login([FromBody] LoginDTO dto)
        {
            var user =await _authService.LoginAsync(dto);
            if(user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            var token = _jwtService.GenerateToken(user.UserId,user.FullName,user.Email,user.Role);
            return Ok(new
            {
                message = "Login successful.",
                token = token,
                user.UserId,
                user.FullName,
                user.Email,
                user.Role
            });

        }

    }
}