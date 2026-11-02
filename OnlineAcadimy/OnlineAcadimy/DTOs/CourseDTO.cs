
namespace OnlineAcadimy.DTOs;

public class CourseDTO
{
    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }
}