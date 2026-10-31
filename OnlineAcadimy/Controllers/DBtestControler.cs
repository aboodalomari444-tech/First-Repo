
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineAcadimy.Models;
//using YourProjectNamespace.Models;

namespace YourProjectNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly OnlineAcademyDbContext _context;

        public DatabaseTestController(OnlineAcademyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            bool connected = await _context.Database.CanConnectAsync();

            if (connected)
                return Ok("Database Connected jb bjbnjnj Successfully!");

            return StatusCode(500, "Could not connect to the database.");
        }
    }
}