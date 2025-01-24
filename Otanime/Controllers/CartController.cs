using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using Otanime.Models;
using System.Text.Json;

namespace Otanime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartItemModel model)
        {
            if (model.ProductId <= 0 || model.Quantity <= 0)
            {
                return BadRequest("Invalid product or quantity.");
            }

            var product = _context.Products.Find(model.ProductId);
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
            if (cart.ContainsKey(model.ProductId))
            {
                cart[model.ProductId] -= model.Quantity;
                if (cart[model.ProductId] <= 0)
                {
                    cart.Remove(model.ProductId);
                }
                SaveCart(cart);
                return Ok("Product removed from cart.");
            }
            return NotFound("Product not found in cart.");
        }

        [HttpGet("items")]
        public IActionResult GetCartItems()
        {
            var cart = GetCart();
            var cartItems = cart.Where(item => _context.Products.Find(item.Key) != null).Select(item => new
            {
                ProductId = item.Key,
                Quantity = item.Value,
                Product = _context.Products.Find(item.Key)
            }).ToList();

            SaveCart(cartItems.ToDictionary(item => item.ProductId, item => item.Quantity));
            return Ok(cartItems);
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

    public class CartItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
