
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class CreateSubmissionDTO
    {
        [StringLength(5000)]
        public string? SubmissionText { get; set; }

       
        [StringLength(2048)]
        public string? FileUrl { get; set; }
    }
}