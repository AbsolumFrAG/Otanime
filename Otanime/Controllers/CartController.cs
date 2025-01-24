using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using System.Text.Json;

namespace Otanime.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController(AppDbContext context) : Controller
{
    [HttpPost("add")]
    public IActionResult AddToCart([FromBody] CartItemModel model)
    {
        if (model.ProductId <= 0 || model.Quantity <= 0)
        {
            return BadRequest("Invalid product or quantity.");
        }

        var product = context.Products.Find(model.ProductId);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        var cart = GetCart();
        if (cart.ContainsKey(model.ProductId))
        {
            cart[model.ProductId] += model.Quantity;
        }
        else
        {
            cart[model.ProductId] = model.Quantity;
        }

        SaveCart(cart);

        return Ok("Product added to cart.");
    }

    [HttpPost("remove")]
    public IActionResult RemoveFromCart([FromBody] CartItemModel model)
    {
        var cart = GetCart();
        if (!cart.ContainsKey(model.ProductId)) return NotFound("Product not found in cart.");
        cart[model.ProductId] -= model.Quantity;
        if (cart[model.ProductId] <= 0)
        {
            cart.Remove(model.ProductId);
        }
        SaveCart(cart);
        return Ok("Product removed from cart.");
    }

    [HttpGet("items")]
    public IActionResult GetCartItems()
    {
        var cart = GetCart();
        var cartItems = cart.Select(item => new
        {
            ProductId = item.Key,
            Quantity = item.Value,
            Product = context.Products.Find(item.Key)
        }).ToList();

        return View(cartItems);
    }

    private Dictionary<int, int> GetCart()
    {
        if (Request.Cookies.TryGetValue("cart", out var cartCookie))
        {
            return JsonSerializer.Deserialize<Dictionary<int, int>>(cartCookie) ?? new Dictionary<int, int>();
        }
        return new Dictionary<int, int>();
    }

    private void SaveCart(Dictionary<int, int> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(7)
        };

        Response.Cookies.Append("cart", cartJson, cookieOptions);
    }
}

public class CartItemModel(int productId, int quantity)
{
    public int ProductId { get; } = productId;
    public int Quantity { get; } = quantity;
}