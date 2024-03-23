using FirebaseAuth.Abstractions;
using FirebaseAuth.Models;
using FirebaseAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FirebaseAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IFirebaseAuthService _authService;

        public AuthController(IFirebaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost, Route("register")]
        [EnableCors("AllowSpecificOrigins")]
        public async Task<IActionResult> Authenticate([FromBody] SignUpUserDto signUpUserDto)
        {
            var token = await _authService.SignUp(signUpUserDto.Email, signUpUserDto.Password);

            if (token is not null)
            {
                Response.Cookies.Append("token", token, new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(3)
                });

                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost, Route("login")]
        [EnableCors("AllowSpecificOrigins")]
        public async Task<IActionResult> Login([FromBody] UserDto UserDto)
        {
            var token = await _authService.Login(UserDto.Email, UserDto.Password);

            if (token is not null)
            {
                Response.Cookies.Append("token", token, new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(3)
                });

                return Ok(new { token });
            }

            return Unauthorized();
        }

        [HttpGet, Route("check")]
        [EnableCors("AllowSpecificOrigins")]
        [Authorize]
        public IActionResult Check()
        {
            if(User.Identity.IsAuthenticated)
            {
                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost, Route("logout")]
        [EnableCors("AllowSpecificOrigins")]
        public IActionResult Logout()
        {
            _authService.SignOut();

            Response.Cookies.Append("token", string.Empty, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(-1)
            });

            return Ok();
        }

    }
}
