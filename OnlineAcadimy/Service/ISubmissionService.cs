using OnlineAcadimy.DTOs;
using OnlineAcadimy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineAcadimy.Services
{
    public interface ISubmissionService
    {
        Task<Submission?> CreateSubmissionAsync(int studentId,int assignmentId,CreateSubmissionDTO dto);

        Task<Submission?> ReviewSubmissionAsync(int instructorId,int submissionId, ReviewSubmissionDTO dto);
        Task<List<MySubmissionDTO>> GetMySubmissionsAsync(int studentId);
        Task<List<SubmissionForInstructorDTO>> GetSubmissionsForInstructorAsync(int instructorId);

    }
}