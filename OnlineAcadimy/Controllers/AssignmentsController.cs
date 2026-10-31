
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAcadimy.Constants;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Services;

namespace OnlineAcadimy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDTO dto)
        {
            var instructorIdClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(instructorIdClaim, out int instructorId))
            {
                return Unauthorized();
            }


            try
            {
                var assignment =
                    await _assignmentService.CreateAssignmentAsync(
                        instructorId, dto);

                if (assignment == null)
                {
                    return NotFound("Course not found.");
                  
                }

                return StatusCode(201, new
                {
                    message = "Assignment created successfully.",
                    assignmentId = assignment.AssignmentId
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }





        [HttpGet("Course/{courseId}")]
        public async Task<IActionResult> GetAssignmentsByCourse(int courseId)
        {
            if (courseId <= 0)
            {
                return BadRequest("Course ID must be greater than zero.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var role = User.FindFirst(AppClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
            {
                return Forbid();
            }

            // 1. Check whether the course exists.
            var courseExists = await _assignmentService
                .CourseExistsAsync(courseId);

            if (!courseExists)
            {
                return NotFound("Course not found.");
            }

            // 2. Check authorization before retrieving assignments.
            var canAccess = await _assignmentService
                .CanAccessCourseAssignmentsAsync(userId, role, courseId);

            if (!canAccess)
            {
                return Forbid();
            }

            // 3. Retrieve assignments only after authorization succeeds.
            var assignments = await _assignmentService
                .GetAssignmentsByCourseAsync(courseId);

            return Ok(assignments);
        }


    }
}