
using Microsoft.EntityFrameworkCore;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly OnlineAcademyDbContext _context;

        public AssignmentService(OnlineAcademyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses
                .AnyAsync(c => c.CourseId == courseId);
        }


        public async Task<Assignment?> CreateAssignmentAsync(int instructorId,CreateAssignmentDTO dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == dto.CourseId);

            if (course == null)
            {
                return null;
            }

            if (dto.DueDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("Due date must be in the future.");
            }

            if (course.InstructorId != instructorId)
            {
                throw new UnauthorizedAccessException();
            }

            var assignment = new Assignment
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate
            };

            _context.Assignments.Add(assignment);

            await _context.SaveChangesAsync();

            return assignment;
        }


        public async Task<List<AssignmentDTO>?> GetAssignmentsByCourseAsync(int courseId)
        {
            var result = await (
                from c in _context.Courses
                where c.CourseId == courseId
                join a in _context.Assignments
                    on c.CourseId equals a.CourseId into courseAssignments
                from a in courseAssignments.DefaultIfEmpty()
                select new
                {
                    Assignment = a == null ? null : new AssignmentDTO
                    {
                        AssignmentId = a.AssignmentId,
                        CourseId = a.CourseId,
                        Title = a.Title,
                        Description = a.Description,
                        DueDate = a.DueDate,
                        CreatedAt = a.CreatedAt
                    }
                }
            ).ToListAsync();

            if (result.Count == 0)
            {
                return null;
            }

            return result
                .Where(x => x.Assignment != null)
                .Select(x => x.Assignment!)
                .ToList();
        }



        public async Task<bool> CanAccessCourseAssignmentsAsync(
            int userId, string role, int courseId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return false;
            }

            if (role == "Instructor")
            {
                return course.InstructorId == userId;
            }

            if (role == "Student")
            {
                return await _context.Enrollments
                    .AnyAsync(e =>
                        e.CourseId == courseId &&
                        e.StudentId == userId &&
                        e.Status == "Active");
            }

            return false;
        }

    }
}