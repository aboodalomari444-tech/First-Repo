
using Microsoft.EntityFrameworkCore;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public class LectureService : ILectureService
    {
        private readonly OnlineAcademyDbContext _context;

        public LectureService(OnlineAcademyDbContext context)
        {
            _context = context;
        }

        public async Task<Lecture?> CreateLectureAsync(int instructorId,CreateLectureDTO dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == dto.CourseId);

            if (course == null)
                return null;

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException();

            var lecture = new Lecture
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                ContentUrl = dto.ContentUrl,
                Description = dto.Description
            };

            _context.Lectures.Add(lecture);
            await _context.SaveChangesAsync();

            return lecture;
        }


        public async Task<List<LectureDTO>?> GetLecturesByCourseAsync(int courseId,int? userId,string? role)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
                return null;

            // Admin can view lectures of any course.
            if (role == "Admin")
            {
                return await GetCourseLecturesAsync(courseId);
            }

            // Instructor can view lectures only for their assigned courses.
            if (role == "Instructor")
            {
                if (userId == null || course.InstructorId != userId)
                    throw new UnauthorizedAccessException(
                        "You can only view lectures for courses assigned to you.");

                return await GetCourseLecturesAsync(courseId);
            }

            // Student can view lectures only if enrolled in a published course.
            if (role == "Student" && userId != null)
            {
                if (!course.IsPublished)
                    throw new UnauthorizedAccessException(
                        "This course is not published.");

                var isEnrolled = await _context.Enrollments
                    .AnyAsync(e =>
                        e.CourseId == courseId &&
                        e.StudentId == userId &&
                        e.Status == "Active");

                if (!isEnrolled)
                    throw new UnauthorizedAccessException(
                        "You must be enrolled in this course to view its lectures.");

                return await GetCourseLecturesAsync(courseId);
            }

            // Unauthenticated users and unsupported roles are not allowed.
            throw new UnauthorizedAccessException(
                "You are not allowed to view these lectures.");
        }

        private async Task<List<LectureDTO>> GetCourseLecturesAsync(int courseId)
        {
            return await _context.Lectures
                .Where(l => l.CourseId == courseId)
                .Select(l => new LectureDTO
                {
                    LectureId = l.LectureId,
                    CourseId = l.CourseId,
                    Title = l.Title,
                    ContentUrl = l.ContentUrl,
                    Description = l.Description,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<Lecture?> UpdateLectureAsync(int lectureId,int instructorId,UpdateLectureDTO dto)
        {
            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LectureId == lectureId);

            if (lecture == null)
                return null;

            if (lecture.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException();

            lecture.Title = dto.Title;
            lecture.ContentUrl = dto.ContentUrl;
            lecture.Description = dto.Description;

            await _context.SaveChangesAsync();

            return lecture;
        }

        public async Task<bool> DeleteLectureAsync(int lectureId,int instructorId)
        {
            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LectureId == lectureId);

            if (lecture == null)
                return false;

            if (lecture.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException();

            _context.Lectures.Remove(lecture);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}