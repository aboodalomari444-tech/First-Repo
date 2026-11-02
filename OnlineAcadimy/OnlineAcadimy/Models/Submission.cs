using System;
using System.Collections.Generic;

namespace OnlineAcadimy.Models;

public partial class Submission
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public int StudentId { get; set; }

    public string? SubmissionText { get; set; }

    public string? FileUrl { get; set; }

    public DateTime SubmittedAt { get; set; }

    public decimal? Grade { get; set; }

    public string? Feedback { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
