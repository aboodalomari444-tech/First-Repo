using System.ComponentModel.DataAnnotations;

namespace OnlineAcadimy.DTOs
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public string Role { get; set; } = null!;
    }
}