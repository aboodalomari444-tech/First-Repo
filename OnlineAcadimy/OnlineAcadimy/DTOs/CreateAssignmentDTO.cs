
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class CreateAssignmentDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        [RegularExpression(@".*\S.*",ErrorMessage = "Title cannot contain only whitespace.")]
        public string Title { get; set; } = string.Empty;


        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}