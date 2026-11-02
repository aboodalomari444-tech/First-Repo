
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Services;
using System.Security.Claims;
using OnlineAcadimy.Constants;

namespace OnlineAcadimy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.AdminOrInstructor)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCourses()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        if (courses == null)
        {
            return NotFound("There is no courses.");
        }

        return Ok(courses);
    }
    [HttpGet("Get By Id/{id}", Name = "GetCourseById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCourseByIdAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid course ID.");
        }

        var course = await _courseService.GetCourseByIdAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        if (!course.IsPublished)
        {
            var role = User.FindFirstValue(AppClaimTypes.Role);

            if (role != Roles.Admin && role != Roles.Instructor)
            {
                return NotFound("Course not found.");
            }
        }

        return Ok(course);
    }
    [HttpPost("Create")]
    [Authorize(Roles =Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]

    public async Task<IActionResult> CreateCourseAsync([FromBody]CreateCourseDTO dto)
    {
        var course=await _courseService.CreateCourseAsync(dto);
        if (course == null)
        {
            return BadRequest("Invalid InstructorId. The instructor does not exist or is not an Instructor.");
        }
        return CreatedAtRoute("GetCourseById", new {id=course.CourseId },course);
    }



    [HttpPut("Update/{id}")]
    [Authorize(Roles = Roles.AdminOrInstructor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourseAsync(int id,[FromBody] UpdateCourseDTO dto)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(AppClaimTypes.Role);

        if (!int.TryParse(userIdValue, out int userId) || role == null)
        {
            return Unauthorized();
        }

        try
        {
            var course = await _courseService.UpdateCourseAsync(
                id, dto, userId, role);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            return Ok(course);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = Roles.AdminOrInstructor)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourseAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid course ID.");
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(AppClaimTypes.Role);

        if (!int.TryParse(userIdValue, out int userId) || role == null)
        {
            return Unauthorized();
        }

        try
        {
            var isDeleted = await _courseService.DeleteCourseAsync(id, userId, role);

            if (!isDeleted)
            {
                return NotFound("Course not found.");
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
    [HttpGet("published")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublishedCourses()
    {
        var courses = await _courseService.GetPublishedCoursesAsync();

        if (courses == null)
        {
            return NotFound("There are no published courses.");
        }

        return Ok(courses);
    }
}