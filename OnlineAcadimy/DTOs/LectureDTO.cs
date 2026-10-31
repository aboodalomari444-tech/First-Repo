
namespace OnlineAcadimy.DTOs
{
    public class LectureDTO
    {
        public int LectureId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ContentUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}