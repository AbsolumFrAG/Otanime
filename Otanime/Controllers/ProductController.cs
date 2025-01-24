using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.Models;

namespace Otanime.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(AppDbContext context) : Controller
{
    private bool IsAdmin()
    {
        if (!Request.Cookies.TryGetValue("userId", out var userIdString) ||
            !int.TryParse(userIdString, out var currentUserId)) return false;
        var currentUser = context.Users.Find(currentUserId);
        return currentUser is { IsAdmin: true };
    }

    [HttpGet("list")]
    public IActionResult List()
    {
        var products = context.Products.Include(p => p.Images).ToList();
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

        context.Products.Add(product);
        context.SaveChanges();

        foreach (var newImage in model.Images.Select(image => new Image
                 {
                     ImageUrl = image.ImageUrl,
                     ProductId = product.ProductId
                 }))
        {
            context.Images.Add(newImage);
        }
        context.SaveChanges();

        return Ok("Product created successfully.");
    }

    [HttpPut("update/{id:int}")]
    public IActionResult Update(int id, [FromBody] ProductModel model)
    {
        if (!IsAdmin())
        {
            return Unauthorized("Only administrators can update products.");
        }

        var product = context.Products.Include(p => p.Images).SingleOrDefault(p => p.ProductId == id);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        product.Name = model.Name;
        product.Price = model.Price;
        product.Description = model.Description;
        product.Stock = model.Stock;

        context.Images.RemoveRange(product.Images);

        foreach (var newImage in model.Images.Select(image => new Image
                 {
                     ImageUrl = image.ImageUrl,
                     ProductId = product.ProductId
                 }))
        {
            context.Images.Add(newImage);
        }

        context.SaveChanges();
        return Ok("Product updated successfully.");
    }

    [HttpDelete("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!IsAdmin())
        {
            return Unauthorized("Only administrators can delete products.");
        }

        var product = context.Products.Include(p => p.Images).SingleOrDefault(p => p.ProductId == id);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        context.Images.RemoveRange(product.Images);
        context.Products.Remove(product);
        context.SaveChanges();
        return Ok("Product deleted successfully.");
    }
}