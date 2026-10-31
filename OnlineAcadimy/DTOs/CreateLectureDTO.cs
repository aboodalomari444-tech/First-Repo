
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class CreateLectureDTO
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? ContentUrl { get; set; }

        public string? Description { get; set; }
    }
}