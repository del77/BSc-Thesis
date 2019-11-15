using System.Threading.Tasks;
using Api.Dtos;
using Api.Framework;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateAccount([FromBody]UserDto user)
        {
            await _userService.Register(user.Username, user.Password);

            return Ok("AccountCreated");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserDto userDto)
        {
            var user = await _userService.Login(userDto.Username, userDto.Password);
            if (user == null)
                throw new ApplicationException("Wrong username or password", "WrongLoginOrPassword");

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Username);
            return Ok(new {token});
        }
    }
}