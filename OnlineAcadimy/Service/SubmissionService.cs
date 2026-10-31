using Microsoft.EntityFrameworkCore;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAcadimy.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly OnlineAcademyDbContext _context;

        public SubmissionService(OnlineAcademyDbContext context)
        {
            _context = context;
        }

        public async Task<Submission?> CreateSubmissionAsync(int studentId,int assignmentId,CreateSubmissionDTO dto)
        {
            // 1. Check whether the assignment exists.
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return null;
            }

            // 2. The student must submit text or a file URL.
            if (string.IsNullOrWhiteSpace(dto.SubmissionText) && string.IsNullOrWhiteSpace(dto.FileUrl))
            {
                throw new ArgumentException("Submission text or file URL is required.");
            }

            // 3. Check whether the student is enrolled in the course.
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e =>e.StudentId == studentId && e.CourseId == assignment.CourseId && e.Status == "Active");

            if (!isEnrolled)
            {
                throw new UnauthorizedAccessException();
            }

            // 4. Reject submissions after the deadline.
            if (assignment.DueDate <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("The assignment deadline has passed.");
            }

            // 5. Create the submission using the authenticated student's ID.
            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                SubmissionText = dto.SubmissionText,
                FileUrl = dto.FileUrl,
                 SubmittedAt = DateTime.UtcNow
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return submission;
        }

        public async Task<Submission?> ReviewSubmissionAsync(int instructorId, int submissionId,ReviewSubmissionDTO dto)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            if (submission == null)
            {
                return null;
            }

            if (submission.Assignment.Course.InstructorId != instructorId)
            {
                throw new UnauthorizedAccessException();
            }


            if (dto.Grade == null && dto.Feedback == null)
            {
                throw new ArgumentException(
                    "Provide a grade or feedback to review the submission.");
            }

            submission.Grade = dto.Grade;
            submission.Feedback = dto.Feedback;
            submission.ReviewedAt = DateTime.UtcNow;


            await _context.SaveChangesAsync();

            return submission;
        }
        public async Task<List<MySubmissionDTO>> GetMySubmissionsAsync(int studentId)
        {
            var submissions = await _context.Submissions
                .Where(s => s.StudentId == studentId)
                .Select(s => new MySubmissionDTO
                {
                    SubmissionId = s.SubmissionId,
                    AssignmentId = s.AssignmentId,
                    AssignmentTitle = s.Assignment.Title,
                    SubmissionText = s.SubmissionText,
                    SubmittedAt = s.SubmittedAt,
                    Grade = s.Grade,
                    Feedback = s.Feedback,
                    ReviewedAt = s.ReviewedAt
                })
                .ToListAsync();

            return submissions;
        }
        public async Task<List<SubmissionForInstructorDTO>> GetSubmissionsForInstructorAsync(int instructorId)
        {
            var submissions = await _context.Submissions
                .Where(s => s.Assignment.Course.InstructorId == instructorId)
                .Select(s => new SubmissionForInstructorDTO
                {
                    SubmissionId = s.SubmissionId,
                    AssignmentId = s.AssignmentId,
                    AssignmentTitle = s.Assignment.Title,
                    StudentId = s.StudentId,
                    StudentName = s.Student.FullName,
                    SubmissionText = s.SubmissionText,
                    SubmittedAt = s.SubmittedAt,
                    Grade = s.Grade,
                    Feedback = s.Feedback,
                    ReviewedAt = s.ReviewedAt
                })
                .ToListAsync();

            return submissions;
        }

    }
}