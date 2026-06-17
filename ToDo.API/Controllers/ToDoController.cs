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

        [HttpPost]
        public async Task<IActionResult> Add(ToDoItemsSaveDto toDoItem)
        {
            await _toDoService.AddAsync(toDoItem);

            return StatusCode(201, $"{toDoItem.content} has been created");
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _toDoService.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _toDoService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompleted([FromQuery] ToDoFilterDto filter)
        {
            var completedItems = await _toDoService.GetCompletedItemsAsync(filter);

            return Ok(completedItems);
        }
 
        [HttpPut("mark/{id:Guid}")]
        public async Task<IActionResult> ToMark(Guid id)
        {
            await _toDoService.ToMarkAsync(id);
            return Ok();
        }

        [HttpPatch("state/{id:Guid}")]
        public async Task<IActionResult> UpdateState(Guid id, [FromBody] ChangeTaskStateDto dto)
        {
            await _toDoService.UpdateState(id, dto);

            return NoContent();
        }

        [HttpPatch("update/{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, ToDoUpdateDto dto)
        {
            await _toDoService.UpdateAsync(id, dto);

            return Ok();
        }

    }
}
