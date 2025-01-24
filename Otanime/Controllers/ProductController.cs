using Microsoft.AspNetCore.Mvc;
using Otanime.Data;
using Otanime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Otanime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            if (Request.Cookies.TryGetValue("userId", out var userIdString) && int.TryParse(userIdString, out var currentUserId))
            {
                var currentUser = _context.Users.Find(currentUserId);
                return currentUser != null && currentUser.IsAdmin;
            }
            return false;
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            var products = _context.Products.Include(p => p.Images).ToList();
            return Ok(products);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] ProductModel model)
        {
            if (!IsAdmin())
            {
                return Unauthorized("Only administrators can create products.");
            }

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Stock = model.Stock
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    var newImage = new Image
                    {
                        ImageUrl = image.ImageUrl,
                        ProductId = product.ProductId
                    };
                    _context.Images.Add(newImage);
                }
                _context.SaveChanges();
            }

            return Ok("Product created successfully.");
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] ProductModel model)
        {
            if (!IsAdmin())
            {
                return Unauthorized("Only administrators can update products.");
            }

            var product = _context.Products.Include(p => p.Images).SingleOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Stock = model.Stock;

            _context.Images.RemoveRange(product.Images);

            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    var newImage = new Image
                    {
                        ImageUrl = image.ImageUrl,
                        ProductId = product.ProductId
                    };
                    _context.Images.Add(newImage);
                }
            }

            _context.SaveChanges();
            return Ok("Product updated successfully.");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                return Unauthorized("Only administrators can delete products.");
            }

            var product = _context.Products.Include(p => p.Images).SingleOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _context.Images.RemoveRange(product.Images);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok("Product deleted successfully.");
        }
    }
}
