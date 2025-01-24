using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.Models;
using Otanime.Models.ViewModels;
using Otanime.Services;

namespace Otanime.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    ApplicationDbContext context,
    ICartService cartService)
    : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly ICartService _cartService = cartService;

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await context.Products
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        var vm = new HomeViewModel
        {
            FeaturedProducts = featuredProducts,
            NewArrivals = await context.Products
                .OrderByDescending(p => p.ReleaseDate)
                .Take(4)
                .ToListAsync()
        };

        return View(vm);
    }

    public async Task<IActionResult> ProductDetails(int id)
    {
        var product = await context.Products
            .Include(p => p.RelatedProducts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        var results = await context.Products
            .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
            .ToListAsync();

        return View(new SearchViewModel
        {
            Query = query,
            Results = results
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}