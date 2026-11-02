
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAcadimy.Constants;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Services;
using System.Security.Claims;

namespace OnlineAcadimy.Controllers
{
   
    [Authorize]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(
            ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        [HttpPost("{assignmentId}")]
        [Authorize(Roles = Roles.Student)]
        public async Task<IActionResult> CreateSubmission(int assignmentId,[FromBody] CreateSubmissionDTO dto)
        {
            if (assignmentId <= 0)
            {
                return BadRequest("Assignment ID must be greater than zero.");
            }

            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(studentIdClaim, out int studentId))
            {
                return Unauthorized();
            }

            try
            {
                var submission = await _submissionService.CreateSubmissionAsync(studentId, assignmentId, dto);

                if (submission == null)
                {
                    return NotFound("Assignment not found.");
                }

                return StatusCode(201, new
                {
                    message = "Submission created successfully.",
                    submissionId = submission.SubmissionId,
                    assignmentId = submission.AssignmentId,
                    submittedAt = submission.SubmittedAt
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        [HttpPut("Review/{submissionId}")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> ReviewSubmission(int submissionId,[FromBody] ReviewSubmissionDTO dto)
        {
            if (submissionId <= 0)
            {
                return BadRequest("Submission ID must be greater than zero.");
            }

            var instructorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           

            if (!int.TryParse(instructorIdClaim, out int instructorId))
            {
                return Unauthorized();
            }

            try
            {
                var submission = await _submissionService.ReviewSubmissionAsync(instructorId,submissionId,dto);

                if (submission == null)
                {
                    return NotFound("Submission not found.");
                }

                return Ok(new
                {
                    message = "Submission reviewed successfully.",
                    submissionId = submission.SubmissionId,
                    grade = submission.Grade,
                    feedback = submission.Feedback,
                    reviewedAt = submission.ReviewedAt
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

        [HttpGet("MySubmissions")]
        [Authorize(Roles = Roles.Student)]
        public async Task<IActionResult> GetMySubmissions()
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(studentIdClaim, out int studentId))
            {
                return Unauthorized();
            }

            var submissions = await _submissionService.GetMySubmissionsAsync(studentId);

            return Ok(submissions);
        }

        [HttpGet("InstructorSubmissions")]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> GetInstructorSubmissions()
        {
            var instructorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(instructorIdClaim, out int instructorId))
            {
                return Unauthorized();
            }

            var submissions = await _submissionService.GetSubmissionsForInstructorAsync(instructorId);

            return Ok(submissions);
        }

    }
}