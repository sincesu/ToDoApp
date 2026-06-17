using System.ComponentModel.DataAnnotations;
using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.ToDo
{
    public class ChangeTaskStateDto
    {
        [Required(ErrorMessage = "State can't be empty")]
        public TaskState State { get; set; }
    }
}
