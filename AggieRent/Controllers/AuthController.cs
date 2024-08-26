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
    [Route("/api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        public enum UserType
        {
            Applicant,
            Owner,
            Admin
        }

        [Route("/login/{type}")]
        [HttpPost]
        public async Task<IActionResult> Login(
            [FromBody] LoginDTO body,
            [FromRoute(Name = "type")] UserType userType
        )
        {
            try
            {
                BaseUser user = userType switch
                {
                    UserType.Applicant => _authService.LoginApplicant(body.Email, body.Password),
                    UserType.Owner => _authService.LoginOwner(body.Email, body.Password),
                    UserType.Admin => _authService.LoginAdmin(body.Email, body.Password),
                    _
                        => throw new ArgumentException(
                            "Unknown user type, must be applicant, owner or admin"
                        )
                };

                // cookie-based authentication
                string claimedRole = userType switch
                {
                    UserType.Applicant => ApplicationConstants.UserRoleClaim.Applicant,
                    UserType.Owner => ApplicationConstants.UserRoleClaim.Owner,
                    UserType.Admin => ApplicationConstants.UserRoleClaim.Admin,
                    _
                        => throw new ArgumentException(
                            "Unknown user type, must be applicant, owner or admin"
                        )
                };
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

        [Route("/logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        [Route("/register/applicant")]
        [HttpPost]
        public IActionResult RegisterApplicant([FromBody] ApplicantRegistrationDTO body)
        {
            try
            {
                _authService.RegisterApplicant(
                    body.Email,
                    body.Password,
                    body.FirstName,
                    body.LastName,
                    body.Gender,
                    body.Birthday,
                    body.Description
                );
                return Created();
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new ErrorResponseDTO(ae));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponseDTO(e));
            }
        }
    }
}
