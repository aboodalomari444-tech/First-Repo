
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class UpdateLectureDTO
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? ContentUrl { get; set; }

        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public string? Description2 { get; set; }

    }
}