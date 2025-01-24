using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using Otanime.Models;

namespace Otanime.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(AppDbContext context) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = context.Users.SingleOrDefault(u => u.Email == model.Email);

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

    [HttpGet]
    public IActionResult Logout()
    {
        if (Request.Cookies["userId"] == null) return RedirectToAction("Index", "Home");
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1)
        };

        Response.Cookies.Append("userId", "", cookieOptions);

        return RedirectToAction("Index", "Home"); 
    }
}