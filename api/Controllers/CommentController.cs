using api.Data;
using api.DTOs.Comment;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/comment/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;
        public CommentController(ICommentRepository commentRepository, IPostRepository postRepository, UserManager<User> userManager)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userManager = userManager;
        }
        
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllComments()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comments = await _commentRepository.GetAllCommentsAsync();
            var commentDtos = comments.Select(c => c.ToCommentDto());
            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var commentModel = await _commentRepository.GetCommentByIdAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }
            return Ok(commentModel.ToCommentDto());
        }

        [HttpPost("{postId:int}/{userId}")]
        //[Authorize]
        public async Task<IActionResult> CreateComment([FromRoute] int postId, [FromRoute] string userId, [FromBody] CreateCommentRequestDto commentDto)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _postRepository.PostExists(postId))
            {
                return BadRequest("Post does not exist");
            }

            if(!await _userManager.Users.AnyAsync(u => u.Id == userId))
            {
                return BadRequest("User does not exist");
            }

            var commentModel = commentDto.ToCommentFromCreateDTO(postId, userId);

            await _commentRepository.CreateCommentAsync(commentModel);

            return CreatedAtAction(nameof(GetCommentById), new { id = commentModel.CommentId }, commentModel.ToCommentDto());
        }

        [HttpPut]
        //[Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentRequestDto updatedCommentDto)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var commentModel = await _commentRepository.UpdateCommentAsync(id, updatedCommentDto.ToCommentFromUpdateDTO());
            if (commentModel == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(commentModel.ToCommentDto());
        }

        [HttpDelete]
        //[Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var commentModel = await _commentRepository.DeleteCommentAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}