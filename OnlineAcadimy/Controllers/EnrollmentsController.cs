
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAcadimy.Constants;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Services;
using System.Security.Claims;

namespace OnlineAcadimy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Student)]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(
            IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        public async Task<IActionResult> EnrollStudent([FromBody] CreateEnrollmentDTO dto)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int studentId))
                return Unauthorized();

            var result = await _enrollmentService
                .EnrollStudentAsync(studentId, dto);

            switch (result)
            {
                case EnrollmentResult.Success:
                    return StatusCode(201, new
                    {
                        message = "Successfully enrolled in the course."
                    });

                case EnrollmentResult.CourseNotFound:
                    return NotFound("Course not found.");

                case EnrollmentResult.AlreadyEnrolled:
                    return Conflict("You are already enrolled in this course.");

                case EnrollmentResult.CourseNotPublished:
                    return Conflict("This course is not published yet.");

                default:
                    return StatusCode(500);
            }
        }
        [HttpGet("MyEnrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int studentId))
                return Unauthorized();

            var enrollments = await _enrollmentService
                .GetMyEnrollmentsAsync(studentId);

            return Ok(enrollments);
        }
        [HttpDelete("{courseId}")]
        [Authorize(Roles = Roles.Student)]
        public async Task<IActionResult> CancelEnrollment(int courseId)
        {
            if (courseId <= 0)
            {
                return BadRequest("Invalid course ID.");
            }

            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(studentIdClaim, out int studentId))
            {
                return Unauthorized();
            }

            var cancelled = await _enrollmentService.CancelEnrollmentAsync(studentId, courseId);

            if (!cancelled)
            {
                return NotFound("Active enrollment not found.");
            }

            return Ok("Enrollment cancelled successfully.");
        }
    }
}