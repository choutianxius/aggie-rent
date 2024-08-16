using AggieRent.DTOs;
using AggieRent.Services;
using Microsoft.AspNetCore.Mvc;

namespace AggieRent.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetUsers().Select(user => new UserSummaryDTO(user)));
        }

        [HttpPost]
        public IActionResult Register([FromBody] UserAuthDTO userAuthDto)
        {
            _userService.Register(userAuthDto.Email, userAuthDto.Password);
            return Ok();
        }
    }
}
