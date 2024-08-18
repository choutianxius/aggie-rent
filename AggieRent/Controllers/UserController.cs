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
        public IActionResult Register([FromBody] UserRegisterDTO userRegisterDto)
        {
            try
            {
                _userService.Register(userRegisterDto.Email, userRegisterDto.Password);
                return Ok();
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ErrorResponseBody(ae.Message));
            }
        }
    }
}
