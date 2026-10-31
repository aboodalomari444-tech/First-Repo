
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs;

public class CreateCourseDTO
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? InstructorId { get; set; }

    public bool IsPublished { get; set; }
}