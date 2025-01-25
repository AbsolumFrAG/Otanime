using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.ViewModels;
using System.Diagnostics;

namespace Otanime.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    ApplicationDbContext context)
    : Controller
{
    public async Task<IActionResult> Index(
        string searchTerm,
        string category,
        string sortBy = "name",
        int page = 1,
        int pageSize = 12)
    {
        var query = context.Products
            .Where(p => p.InStock)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Description.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        query = sortBy.ToLower() switch
        {
            "price" => query.OrderBy(p => (double)p.Price),
            "price_desc" => query.OrderByDescending(p => (double)p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        var totalItems = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new HomeViewModel
        {
            Products = products,
            Categories = await context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync(),
            SearchTerm = searchTerm,
            SelectedCategory = category,
            SortBy = sortBy,
            Pagination = new PaginationInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalItems
            }
        };

        return View(vm);
    }
}