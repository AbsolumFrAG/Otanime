using Microsoft.AspNetCore.Mvc;
using Otanime.Data;

namespace Otanime.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(AppDbContext context) : Controller
{
    private static bool ValidatePassword(string password)
    {
        if (password.Length < 6)
            return false;

        var containsUpper = password.Any(char.IsUpper);
        var containsLower = password.Any(char.IsLower);
        var containsDigit = password.Any(char.IsDigit);

        return containsUpper && containsLower && containsDigit;
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] User model)
    {
        if (context.Users.Any(u => u.Email == model.Email))
        {
            return BadRequest("Email is already in use.");
        }

        if (!ValidatePassword(model.Password))
        {
            return BadRequest("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");
        }

        model.SetPassword(model.Password);
        model.IsAdmin = false;
        context.Users.Add(model);
        context.SaveChanges();

        return Ok("User created successfully.");
    }

    [HttpDelete("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!Request.Cookies.TryGetValue("userId", out var userIdString) ||
            !int.TryParse(userIdString, out var currentUserId)) return Unauthorized("User not authenticated.");
        var currentUser = context.Users.Find(currentUserId);

        if (currentUser == null || (!currentUser.IsAdmin && currentUser.UserId != id))
        {
            return Unauthorized("You do not have permission to delete this account.");
        }

        var userToDelete = context.Users.Find(id);

        if (userToDelete == null)
        {
            return NotFound("User not found.");
        }

        context.Users.Remove(userToDelete);
        context.SaveChanges();

        if (currentUser.UserId != id) return Ok("User deleted successfully.");
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1)
        };
        Response.Cookies.Append("userId", "", cookieOptions);

        return Ok("User deleted successfully.");
    }
}