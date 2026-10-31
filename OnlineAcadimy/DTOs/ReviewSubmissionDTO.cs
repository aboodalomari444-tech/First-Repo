
using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class ReviewSubmissionDTO
    {
       
        [Range(0, 100)]
        public decimal? Grade { get; set; }

        [StringLength(2000)]
        public string? Feedback { get; set; }
    }
}
