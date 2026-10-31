using System;

namespace OnlineAcadimy.DTOs
{
    public class MySubmissionDTO
    {
        public int SubmissionId { get; set; }

        public int AssignmentId { get; set; }

        public string AssignmentTitle { get; set; } = null!;

        public string? SubmissionText { get; set; }

        public DateTime SubmittedAt { get; set; }

        public decimal? Grade { get; set; }

        public string? Feedback { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}