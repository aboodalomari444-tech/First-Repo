
using Microsoft.EntityFrameworkCore;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly OnlineAcademyDbContext _context;

        public EnrollmentService(OnlineAcademyDbContext context)
        {
            _context = context;
        }

        public async Task<EnrollmentResult> EnrollStudentAsync(int studentId,CreateEnrollmentDTO dto)
        {
            // 1. Check whether the course exists
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == dto.CourseId);

            if (course == null)
                return EnrollmentResult.CourseNotFound;

            // 2. Check whether the course is published
            if (!course.IsPublished)
                return EnrollmentResult.CourseNotPublished;

            // 3. Check whether the student is already enrolled
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.StudentId == studentId &&
                    e.CourseId == dto.CourseId);

            if (alreadyEnrolled)
                return EnrollmentResult.AlreadyEnrolled;

            // 4. Create the enrollment
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = dto.CourseId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return EnrollmentResult.Success;
        }
        public async Task<List<MyEnrollmentDTO>> GetMyEnrollmentsAsync(int studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => new MyEnrollmentDTO
                {
                    EnrollmentId = e.EnrollmentId,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    CourseDescription = e.Course.Description,
                    EnrolledAt = e.EnrolledAt,
                    Status = e.Status
                })
                .ToListAsync();
        }
        public async Task<bool> CancelEnrollmentAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e =>e.StudentId == studentId && e.CourseId == courseId && e.Status == "Active");

            if (enrollment == null)
            {
                return false;
            }

            enrollment.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}