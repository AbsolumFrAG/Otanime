using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using Microsoft.EntityFrameworkCore;

namespace Otanime.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }
    private static bool ValidatePassword(string password)
    {
        if (password.Length < 6)
            return false;

        var containsUpper = password.Any(char.IsUpper);
        var containsLower = password.Any(char.IsLower);
        var containsDigit = password.Any(char.IsDigit);

        return containsUpper && containsLower && containsDigit;
    }

    [HttpGet("{id}")]
    public IActionResult GetUserOrders(int id)
    {
        if (Request.Cookies.TryGetValue("userId", out var userIdString) && int.TryParse(userIdString, out var currentUserId))
        {
            var currentUser = _context.Users
                .Include(u => u.Orders!)
                .ThenInclude(o => o.ProductOrders!)
                .ThenInclude(po => po.Product)
                .FirstOrDefault(u => u.UserId == currentUserId);

            if (currentUser == null || (!currentUser.IsAdmin && currentUser.UserId != id))
            {
                return Unauthorized("You do not have permission to view this user's orders.");
            }

            var user = _context.Users
                .Include(u => u.Orders!)
                .ThenInclude(o => o.ProductOrders!)
                .ThenInclude(po => po.Product)
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Address,
                user.City,
                user.Country,
                user.PostalCode,
                user.Phone,
                Orders = user.Orders?.Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.PaymentMethod,
                    o.PaymentStatus,
                    o.Status,
                    o.DeliveryType,
                    o.DeliveryPrice,
                    o.Address,
                    o.City,
                    o.Country,
                    o.PostalCode,
                    o.Phone,
                    ProductOrders = o.ProductOrders.Select(po => new
                    {
                        po.ProductOrderId,
                        po.ProductId,
                        po.Product.Name,
                        po.Quantity,
                        po.Price,
                        po.DeliveryDate
                    })
                })
            });
        }

        return Unauthorized("User not authenticated.");
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

    [HttpDelete("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!Request.Cookies.TryGetValue("userId", out var userIdString) ||
            !int.TryParse(userIdString, out var currentUserId)) return Unauthorized("User not authenticated.");
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

        if (currentUser.UserId != id) return Ok("User deleted successfully.");
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1)
        };
        Response.Cookies.Append("userId", "", cookieOptions);

        return Ok("User deleted successfully.");
    }
}