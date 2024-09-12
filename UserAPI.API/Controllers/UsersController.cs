using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserAPI.API.Models;
using UserAPI.DataAccess;
using UserAPI.DataAccess.Repository;

namespace UserAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        protected readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return NotFound();
            }
            var userDto = UserMapper.ToDto(user);
            return Ok(userDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            if (users.Any())
            {
                var userDtos = users.Select(UserMapper.ToDto).ToList();
                return Ok(userDtos);
            }
            _logger.LogWarning("Users not found");
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            var user = UserMapper.ToEntity(userDto);
            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var existingUser = await _userRepository.GetUserAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (! ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = UserMapper.ToEntity(userDto);

            await _userRepository.AddUserAsync(user);
            var createdDto = UserMapper.ToDto(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, createdDto);
        }
    }
}
