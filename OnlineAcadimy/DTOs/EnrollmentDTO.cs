
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class CreateEnrollmentDTO
    {
        [Required]
        public int CourseId { get; set; }
    }
}