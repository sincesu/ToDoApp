using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.ToDo;
using ToDo.Application.DTOs.Filter;

namespace ToDo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ToDoFilterDto? filter)
        {
            return Ok(await _toDoService.GetItemsAsync(filter));
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _toDoService.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPatch("assigntask")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTask(AssignTaskDto dto)
        {
            await _toDoService.AssignTask(dto);
            return Ok();
        }

        [HttpGet("history/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTaskHistories(Guid id)
        {
            var histories = await _toDoService.GetTaskHistoriesAsync(id);
            return Ok(histories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(ToDoItemsSaveDto toDoItem)
        {
            await _toDoService.AddAsync(toDoItem);

            return StatusCode(201, $"{toDoItem.content} has been created");
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompleted([FromQuery] ToDoFilterDto filter)
        {
            var completedItems = await _toDoService.GetCompletedItemsAsync(filter);

            return Ok(completedItems);
        }
 
        [HttpPut("mark/{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToMarkForCompleted(Guid id)
        {
            await _toDoService.ToMarkForCompletedAsync(id);
            return Ok();
        }

        [HttpPatch("update/{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, ToDoUpdateDto dto)
        {
            await _toDoService.UpdateAsync(id, dto);

            return Ok();
        }

        [HttpPatch("state/{id:Guid}")]
        public async Task<IActionResult> UpdateState(Guid id, [FromBody] ChangeTaskStateDto dto)
        {
            await _toDoService.UpdateState(id, dto);

            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _toDoService.DeleteAllCommentsOfTaskAsync(id);
            return NoContent();
        }

    }
}
