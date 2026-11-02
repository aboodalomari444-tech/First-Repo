
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public interface IAssignmentService
    {
        Task<Assignment?> CreateAssignmentAsync(int instructorId,CreateAssignmentDTO dto);

        Task<List<AssignmentDTO>?> GetAssignmentsByCourseAsync(int courseId);

        Task<bool> CourseExistsAsync(int courseId);

        Task<bool> CanAccessCourseAssignmentsAsync(int userId,string role,int courseId);
    }
}
