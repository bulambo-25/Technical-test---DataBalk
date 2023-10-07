using System.ComponentModel.DataAnnotations;

namespace Workflow.API.Models.Dto
{
    public class TasksDto
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }
        public required User UserId { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}
