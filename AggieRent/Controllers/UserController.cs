using AggieRent.Common;
using AggieRent.DTOs;
using AggieRent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AggieRent.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        [Authorize(Roles = ApplicationConstants.UserRoleClaim.Admin)]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetUsers().Select(user => new UserSummaryDTO(user)));
        }
    }
}
