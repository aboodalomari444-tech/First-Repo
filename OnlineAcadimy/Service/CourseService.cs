
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;
using System.Linq;


namespace OnlineAcadimy.Services;

public class CourseService : ICourseService
{
    private readonly OnlineAcademyDbContext _context;

    public CourseService(OnlineAcademyDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseDTO>> GetAllCoursesAsync()
    {
        return await _context.Courses
            .Select(course => new CourseDTO
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                IsPublished = course.IsPublished,
                CreatedAt = course.CreatedAt
            })
            .ToListAsync();
    }
    public async Task<CourseDTO?> GetCourseByIdAsync(int id)
    {
        var course= await _context.Courses.Where(c=>c.CourseId == id).Select(c=>new CourseDTO
        {
            CourseId=c.CourseId,
            Title = c.Title,
            Description =c.Description,
            IsPublished=c.IsPublished,
            CreatedAt = c.CreatedAt,
        }).FirstOrDefaultAsync();
        return course;
    }
    public async Task<CourseDTO?> CreateCourseAsync(CreateCourseDTO dto)
    {


        if (dto.InstructorId.HasValue)
        {
            var instructor = await _context.Users
                .FindAsync(dto.InstructorId.Value);

            if (instructor == null || instructor.Role != "Instructor")
            {
                return null;
            }
        }
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            InstructorId = dto.InstructorId,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return new CourseDTO
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt
        };
       
    }
    public async Task<CourseDTO?> UpdateCourseAsync(int id,UpdateCourseDTO dto,int userId,string role)
    {
        var course=await _context.Courses.Where(s=>s.CourseId==id).FirstOrDefaultAsync();
        if (course == null)
        {
            return null;
        }

        if (role == "Instructor" && course.InstructorId != userId)
        {
            throw new UnauthorizedAccessException(
                "You can only update courses assigned to you.");
        }
        course.Title= dto.Title;
        course.Description= dto.Description;
        if (role == "Admin")
        {
            course.IsPublished = dto.IsPublished;
        }
        await _context.SaveChangesAsync();
        return new CourseDTO
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt
        };
       
    }
    public async Task<bool> DeleteCourseAsync(int id, int userId, string role)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return false;
        }

        if (role == "Instructor" && course.InstructorId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete courses assigned to you.");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<List<CourseDTO>> GetPublishedCoursesAsync()
    {
        return await _context.Courses
            .Where(course => course.IsPublished)
            .Select(course => new CourseDTO
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                IsPublished = course.IsPublished,
                CreatedAt = course.CreatedAt
            })
            .ToListAsync();
    }
}