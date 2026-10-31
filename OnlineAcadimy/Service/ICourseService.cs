
using OnlineAcadimy.DTOs;

namespace OnlineAcadimy.Services;

public interface ICourseService
{
    Task<List<CourseDTO>> GetAllCoursesAsync();
    Task<CourseDTO?> GetCourseByIdAsync(int id);
    Task<CourseDTO?> CreateCourseAsync(CreateCourseDTO dto);
    Task<CourseDTO?> UpdateCourseAsync( int id,UpdateCourseDTO dto,int userId,string role);
    Task<bool> DeleteCourseAsync(int id, int userId, string role);
    Task<List<CourseDTO>> GetPublishedCoursesAsync();
}