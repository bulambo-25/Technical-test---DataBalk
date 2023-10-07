using System.ComponentModel.DataAnnotations;

namespace Workflow.API.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(16)]
        public string Password { get; set; }
    }
}
