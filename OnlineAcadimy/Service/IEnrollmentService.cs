
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public enum EnrollmentResult
    {
        Success,
        CourseNotFound,
        AlreadyEnrolled,
        CourseNotPublished
    }


    public interface IEnrollmentService
    {  

        Task<EnrollmentResult> EnrollStudentAsync(int studentId,DTOs.CreateEnrollmentDTO dto);
        Task<List<MyEnrollmentDTO>> GetMyEnrollmentsAsync(int studentId);
        Task<bool> CancelEnrollmentAsync(int studentId, int courseId);
    }
}