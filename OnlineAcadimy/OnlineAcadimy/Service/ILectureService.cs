
using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;

namespace OnlineAcadimy.Services
{
    public interface ILectureService
    {
        Task<Lecture?> CreateLectureAsync(int instructorId,CreateLectureDTO dto);

        Task<List<LectureDTO>?> GetLecturesByCourseAsync(int courseId, int? userId, string? role);

        Task<Lecture?> UpdateLectureAsync(int lectureId,int instructorId,UpdateLectureDTO dto);

        Task<bool> DeleteLectureAsync(int lectureId,int instructorId);
    }
}