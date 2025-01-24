using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using System.Text.Json;

namespace Otanime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        private int? GetUserIdFromCookie()
        {
            if (Request.Cookies.TryGetValue("userId", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return null;
        }

        private Dictionary<int, int> GetCart()
        {
            if (Request.Cookies.TryGetValue("cart", out var cartCookie))
            {
                return JsonSerializer.Deserialize<Dictionary<int, int>>(cartCookie) ?? new Dictionary<int, int>();
            }
            return new Dictionary<int, int>();
        }

        [HttpPost("place-order")]
        public IActionResult PlaceOrder([FromBody] OrderModel orderModel)
        {
            var userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            if (string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.City) || string.IsNullOrEmpty(user.Country) || string.IsNullOrEmpty(user.PostalCode) || string.IsNullOrEmpty(user.Phone))
            {
                user.Address = !string.IsNullOrEmpty(orderModel.BillingAddress) ? orderModel.BillingAddress : user.Address;
                user.City = !string.IsNullOrEmpty(orderModel.BillingCity) ? orderModel.BillingCity : user.City;
                user.Country = !string.IsNullOrEmpty(orderModel.BillingCountry) ? orderModel.BillingCountry : user.Country;
                user.PostalCode = !string.IsNullOrEmpty(orderModel.BillingPostalCode) ? orderModel.BillingPostalCode : user.PostalCode;
                user.Phone = !string.IsNullOrEmpty(orderModel.BillingPhone) ? orderModel.BillingPhone : user.Phone;

                _context.SaveChanges();

                if (string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.City) || string.IsNullOrEmpty(user.Country) || string.IsNullOrEmpty(user.PostalCode) || string.IsNullOrEmpty(user.Phone))
                {
                    return BadRequest("Complete billing address is required in the account.");
                }
            }

            var cart = GetCart();
            if (!cart.Any())
            {
                return BadRequest("Your cart is empty.");
            }

            var order = new Order
            {
                UserId = user.UserId,
                User = user,
                OrderDate = DateTime.Now,
                PaymentMethod = "Card",
                PaymentStatus = "Success",
                Status = "Processing",
                DeliveryType = orderModel.DeliveryType,
                DeliveryPrice = orderModel.DeliveryType == "Shipping" ? orderModel.DeliveryPrice : "0",
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                PostalCode = user.PostalCode,
                Phone = user.Phone
            };

            if(orderModel.ShippingAddress != null)
            {
                order.Address = orderModel.ShippingAddress;
            }

            if(orderModel.ShippingCity != null)
            {
                order.City = orderModel.ShippingCity;
            }

            if(orderModel.ShippingCountry != null)
            {
                order.Country = orderModel.ShippingCountry;
            }

            if(orderModel.ShippingPostalCode != null)
            {
                order.PostalCode = orderModel.ShippingPostalCode;
            }

            if(orderModel.ShippingPhone != null)
            {
                order.Phone = orderModel.ShippingPhone;
            }

            foreach (var item in cart)
            {
                var product = _context.Products.Find(item.Key);
                if (product == null)
                {
                    return NotFound($"Product with ID {item.Key} not found.");
                }

                if (product.Stock < item.Value)
                {
                    return BadRequest($"Insufficient stock for product {product.Name}.");
                }

                product.Stock -= item.Value;

                var productOrder = new ProductOrder
                {
                    ProductId = product.ProductId,
                    Product = product,
                    Quantity = item.Value,
                    Price = product.Price,
                    DeliveryDate = DateTime.Now.AddDays(3),
                    Order = order
                };

                order.ProductOrders.Add(productOrder);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            Response.Cookies.Delete("cart");

            return Ok("Order placed successfully.");
        }
    }

    public class OrderModel
    {
        public required string DeliveryType { get; set; }
        public required string DeliveryPrice { get; set; }
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = string.Empty;
        public string BillingPostalCode { get; set; } = string.Empty;
        public string BillingPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public string ShippingPostalCode { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
    }
}
