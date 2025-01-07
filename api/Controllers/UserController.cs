using System.Security.Claims;
using api.Data;
using api.DTOs.User;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace api.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signinManager;

        public UserController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signinManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signinManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _userManager.Users.ToListAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
            }).ToList();
            return Ok(userDtos);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userModel = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (userModel == null)
            {
                return NotFound("User not found");
            }

            return Ok(
                new UserDto
                {
                    Id = userModel.Id,
                    UserName = userModel.UserName ?? string.Empty,
                    Email = userModel.Email ?? string.Empty,
                }
            );
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto requestDtoDto)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userModel = new User
                {
                    UserName = requestDtoDto.Username,
                    Email = requestDtoDto.Email,
                    ProfilePicture = requestDtoDto.ProfilePicture
                };

                var result = await _userManager.CreateAsync(userModel, requestDtoDto.Password);

                if(result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(userModel, "User");
                    if(roleResult.Succeeded)
                    {
                        return Ok(
                            new UserDto
                            {
                                UserName = userModel.UserName ?? string.Empty,
                                Email = userModel.Email ?? string.Empty,
                                Token = _tokenService.CreateToken(userModel)
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, result.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userModel = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

            if (userModel == null) return Unauthorized("Invalid username!");

            var result = await _signinManager.CheckPasswordSignInAsync(userModel, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            return Ok(
                new UserDto
                {
                    Id = userModel.Id,
                    UserName = userModel.UserName ?? string.Empty,
                    Email = userModel.Email ?? string.Empty,
                    Token = _tokenService.CreateToken(userModel)
                }
            );
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequestDto updatedUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the logged-in user's ID
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the logged-in user's ID matches the ID of the user being edited
            if (loggedInUserId != id)
            {
                return Forbid();
            }

            var userModel = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (userModel == null)
            {
                return NotFound();
            }

            userModel.UserName = updatedUserDto.Username;
            userModel.PasswordHash = _userManager.PasswordHasher.HashPassword(userModel, updatedUserDto.Password);
            userModel.Email = updatedUserDto.Email;
            userModel.ProfilePicture = updatedUserDto.ProfilePicture;

            var result = await _userManager.UpdateAsync(userModel);

            if (result.Succeeded)
            {
                return Ok(
                    new UserDto
                    {
                        UserName = userModel.UserName ?? string.Empty,
                        Email = userModel.Email ?? string.Empty,
                        Token = _tokenService.CreateToken(userModel)
                    }
                );
            }
            else
            {
                return StatusCode(500, result.Errors);
            }
        }
    }
}
