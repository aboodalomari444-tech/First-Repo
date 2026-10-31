
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
    [Authorize]
    public class LecturesController : ControllerBase
    {
        private readonly ILectureService _lectureService;

        public LecturesController(ILectureService lectureService)
        {
            _lectureService = lectureService;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> CreateLecture([FromBody] CreateLectureDTO dto)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int instructorId))
                return Unauthorized();

            try
            {
                var lecture = await _lectureService
                    .CreateLectureAsync(instructorId, dto);

                if (lecture == null)
                    return NotFound("Course not found.");

                return StatusCode(201, new
                {
                    message = "Lecture created successfully.",
                    lectureId = lecture.LectureId
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpGet("Course/{courseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLecturesByCourse(int courseId)
        {
            if (courseId <= 0)
                return BadRequest("Course ID must be greater than zero.");

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(AppClaimTypes.Role);

            int? userId = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                if (!int.TryParse(userIdValue, out int parsedUserId) || role == null)
                    return Unauthorized();

                userId = parsedUserId;
            }

            try
            {
                var lectures = await _lectureService
                    .GetLecturesByCourseAsync(courseId, userId, role);

                if (lectures == null)
                    return NotFound("Course not found.");

                return Ok(lectures);
            }
            catch (UnauthorizedAccessException)
            {
                if (User.Identity?.IsAuthenticated != true)
                    return Unauthorized();

                return Forbid();
            }
        }

        [HttpPut("{lectureId}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> UpdateLecture(int lectureId,[FromBody] UpdateLectureDTO dto)
        {
            if (lectureId <= 0)
                return BadRequest("Lecture ID must be greater than zero.");

            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int instructorId))
                return Unauthorized();

            try
            {
                var lecture = await _lectureService.UpdateLectureAsync(
                    lectureId,
                    instructorId,
                    dto);

                if (lecture == null)
                    return NotFound("Lecture not found.");

                return Ok(new
                {
                    message = "Lecture updated successfully.",
                    lectureId = lecture.LectureId
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{lectureId}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> DeleteLecture(int lectureId)
        {
            if (lectureId <= 0)
                return BadRequest("Lecture ID must be greater than zero.");

            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int instructorId))
                return Unauthorized();

            try
            {
                var deleted = await _lectureService.DeleteLectureAsync(
                    lectureId,
                    instructorId);

                if (!deleted)
                    return NotFound("Lecture not found.");

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }

}