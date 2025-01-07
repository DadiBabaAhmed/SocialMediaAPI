using api.Data;
using api.DTOs.Post;
using api.Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Helpers;
using Microsoft.AspNetCore.Identity;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using api.Extensions;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/post/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;
        public PostController(IPostRepository postRepository, UserManager<User> userManager)
        {
            _postRepository = postRepository;
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] QueryObject queryObject)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var posts = await _postRepository.GetAllPostsAsync(queryObject);
            var postDtos = posts.Select(p => p.ToPostDto()).ToList();
            return Ok(postDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPostById(int id)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var postModel = await _postRepository.GetPostByIdAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            return Ok(postModel.ToPostDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDto postDto)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var postModel = postDto.ToPostFromCreateDTO(userId);

            await _postRepository.CreatePostAsync(postModel);

            var createdPost = await _postRepository.GetPostByIdAsync(postModel.PostId);
            if(createdPost == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetPostById), new { id = postModel.PostId }, createdPost.ToPostDto());
        }

        [HttpPut]
        [Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostRequestDto updatedPostDto)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (updatedPostDto == null)
            {
                return BadRequest();
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var verifPost = await _postRepository.GetPostByIdAsync(id);

            if (verifPost == null)
            {
                return NotFound();
            }

            if (verifPost.UserId != loggedInUserId)
            {
                return Forbid();
            }

            var postModel = await _postRepository.UpdatePostAsync(id, updatedPostDto);
            if (postModel == null)
            {
                return NotFound();
            }
            return Ok(postModel.ToPostDto());
        }

        [HttpDelete]
        [Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var verifPost = await _postRepository.GetPostByIdAsync(id);

            if (verifPost == null)
            {
                return NotFound();
            }

            if (verifPost.UserId != loggedInUserId)
            {
                return Forbid();
            }

            if (verifPost.UserId != loggedInUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var postModel = await _postRepository.DeletePostAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}