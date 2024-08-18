using System.Security.Claims;
using AggieRent.Common;
using AggieRent.DTOs;
using AggieRent.Models;
using AggieRent.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AggieRent.Controllers
{
    [ApiController]
    public class AuthController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Route("/api/register")]
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
                return BadRequest(new ErrorResponseDTO(ae.Message));
            }
        }

        [Route("/api/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDto)
        {
            try
            {
                var user = _userService.Login(userLoginDto.Email, userLoginDto.Password);

                // cookie-based authentication
                string claimedRole = user.Role.Equals(UserRole.Admin)
                    ? ApplicationConstants.UserRoleClaim.Admin
                    : ApplicationConstants.UserRoleClaim.User;
                var claims = new List<Claim>()
                {
                    new(ClaimTypes.Name, user.Email),
                    new(ClaimTypes.Role, claimedRole),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return Ok();
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ErrorResponseDTO(ae.Message));
            }
        }
    }
}
