namespace OnlineAcadimy.DTOs
{
    public class SubmissionForInstructorDTO
    {
        public int SubmissionId { get; set; }

        public int AssignmentId { get; set; }

        public string AssignmentTitle { get; set; } = null!;

        public int StudentId { get; set; }

        public string StudentName { get; set; } = null!;

        public string? SubmissionText { get; set; }

        public DateTime SubmittedAt { get; set; }

        public decimal? Grade { get; set; }

        public string? Feedback { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}