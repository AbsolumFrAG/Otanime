using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using Otanime.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Otanime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 6)
                return false;

            bool containsUpper = password.Any(char.IsUpper);
            bool containsLower = password.Any(char.IsLower);
            bool containsDigit = password.Any(char.IsDigit);

            return containsUpper && containsLower && containsDigit;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] User model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest("Email is already in use.");
            }

            if (!ValidatePassword(model.Password))
            {
                return BadRequest("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");
            }

            model.SetPassword(model.Password);
            model.IsAdmin = false;
            _context.Users.Add(model);
            _context.SaveChanges();

            return Ok("User created successfully.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (Request.Cookies.TryGetValue("userId", out var userIdString) && int.TryParse(userIdString, out var currentUserId))
            {
                var currentUser = _context.Users.Find(currentUserId);

                if (currentUser == null || (!currentUser.IsAdmin && currentUser.UserId != id))
                {
                    return Unauthorized("You do not have permission to delete this account.");
                }

                var userToDelete = _context.Users.Find(id);

                if (userToDelete == null)
                {
                    return NotFound("User not found.");
                }

                _context.Users.Remove(userToDelete);
                _context.SaveChanges();

                if (currentUser.UserId == id)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    };
                    Response.Cookies.Append("userId", "", cookieOptions);
                }

                return Ok("User deleted successfully.");
            }

            return Unauthorized("User not authenticated.");
        }


    }
}
