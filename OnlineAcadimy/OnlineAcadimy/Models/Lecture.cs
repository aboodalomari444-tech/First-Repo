using System;
using System.Collections.Generic;

namespace OnlineAcadimy.Models;

public partial class Lecture
{
    public int LectureId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? ContentUrl { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;
}
