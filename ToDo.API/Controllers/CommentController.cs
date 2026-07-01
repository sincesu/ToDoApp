using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Abstractions;
using ToDo.Application.DTOs.Comment;

namespace ToDo.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("task/{taskId:guid}")]
        public async Task<IActionResult> GetAllCommentsByTask(Guid taskId)
        {
            return Ok(await _commentService.GetAllCommentsForTask(taskId));
        }

        [HttpGet("myComments")]
        public async Task<IActionResult> GetAllMyComments()
        {
            return Ok(await _commentService.GetAllMyComments());
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            return Ok(await _commentService.GetCommentForId(id));
        }

        [HttpGet("user/{id:Guid}")]
        public async Task<IActionResult> GetCommentsByUserId(Guid id)
        {
            return Ok(await _commentService.GetCommentsForUser(id));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddComment([FromForm] CommentSaveDto dto)
        {
            await _commentService.AddComment(dto);
            return StatusCode(201, "Comment has been created");
        }

        [HttpPut("update/{id:Guid}")]
        public async Task<IActionResult> UpdateComment(Guid id, CommentUpdateDto dto)
        {
            await _commentService.UpdateComment(id, dto);

            return Ok();
        }

        [HttpDelete("delete/{id:Guid}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            await _commentService.DeleteComment(id);
            return NoContent();
        }

    }
}
