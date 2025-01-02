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

namespace api.Controllers
{
    [Route("api/post")]
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
        [Authorize]
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
            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post.ToPostDto());
        }

        [HttpPost]
        [Authorize]
        [Route("{Userid}")]
        public async Task<IActionResult> CreatePost([FromRoute] string Userid,[FromBody] CreatePostRequestDto postDto)
        {
             if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!await _userManager.Users.AnyAsync(u => u.Id == Userid))
            {
                return BadRequest("User does not exist");
            }

            var postModel = postDto.ToPostFromCreateDTO(Userid);
            await _postRepository.CreatePostAsync(postModel);
            return CreatedAtAction(nameof(GetPostById), new { id = postModel.PostId }, postModel.ToPostDto());
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
            var postModel = await _postRepository.DeletePostAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}