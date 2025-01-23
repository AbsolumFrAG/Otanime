using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using Otanime.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Otanime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);

            if (user == null || !user.VerifyPassword(model.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Append("userId", user.UserId.ToString(), cookieOptions);

            return Ok("Login successful.");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies["userId"] != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                
                Response.Cookies.Append("userId", "", cookieOptions);
            }

            return Ok("Logout successful.");
        }
    }
}
