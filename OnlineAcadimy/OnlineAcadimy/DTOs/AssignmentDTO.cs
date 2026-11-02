
namespace OnlineAcadimy.DTOs
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}