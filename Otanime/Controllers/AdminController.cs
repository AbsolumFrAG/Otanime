using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.Models;
using Otanime.ViewModels;

namespace Otanime.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<AdminController> logger)
    : Controller
{
    public IActionResult Index()
    {
        var dashboardModel = new AdminDashboardViewModel
        {
            TotalProducts = context.Products.Count(),
            ProductsOutOfStock = context.Products.Count(p => !p.InStock || p.StockQuantity == 0),
            TotalOrders = context.Orders.Count(),
            PendingOrders = context.Orders.Count(o => o.Status == OrderStatus.Pending),
            TotalUsers = userManager.Users.Count(),
            RecentOrders = context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList(),
            LowStockProducts = context.Products
                .Where(p => p.StockQuantity <= 10 && p.InStock)
                .Take(5)
                .ToList()
        };

        return View(dashboardModel);
    }
    #region Produits

    // GET: Admin/Products
    public IActionResult Products(string search = "", string category = "", string sortBy = "name", int page = 1, int pageSize = 10)
    {
        try
        {
            var query = context.Products.AsQueryable();

            // Filtrage
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(p => 
                    p.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) || 
                    p.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // Tri
            query = sortBy switch
            {
                "price" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "stock" => query.OrderBy(p => p.StockQuantity),
                _ => query.OrderBy(p => p.Name)
            };

            // Pagination
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBag data
            ViewBag.Categories = context.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.SortBy = sortBy;

            return View(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors du chargement de la liste des produits");
            throw;
        }
    }

    // GET: Admin/CreateProduct
    public IActionResult CreateProduct()
    {
        return View(new ProductCreateEditViewModel());
    }

    // POST: Admin/CreateProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateProduct(ProductCreateEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                Category = model.Category,
                StockQuantity = model.StockQuantity,
                InStock = model.InStock
            };

            context.Products.Add(product);
            context.SaveChanges();

            TempData["Success"] = "Produit créé avec succès";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la création du produit");
            ModelState.AddModelError("", "Une erreur est survenue lors de la création du produit");
            return View(model);
        }
    }

    // GET: Admin/EditProduct/5
    public IActionResult EditProduct(int id)
    {
        try
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductCreateEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                StockQuantity = product.StockQuantity,
                InStock = product.InStock
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors du chargement du produit à éditer");
            throw;
        }
    }

    // POST: Admin/EditProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditProduct(int id, ProductCreateEditViewModel model)
    {
        try
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.Category = model.Category;
            product.StockQuantity = model.StockQuantity;
            product.InStock = model.InStock;

            context.SaveChanges();

            TempData["Success"] = "Produit mis à jour avec succès";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la mise à jour du produit");
            ModelState.AddModelError("", "Une erreur est survenue lors de la mise à jour du produit");
            return View(model);
        }
    }

    // POST: Admin/DeleteProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return RedirectToAction(nameof(Products));
            context.Products.Remove(product);
            context.SaveChanges();
            TempData["Success"] = "Produit supprimé avec succès";

            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la suppression du produit");
            TempData["Error"] = "Une erreur est survenue lors de la suppression du produit";
            return RedirectToAction(nameof(Products));
        }
    }

    #endregion

    #region Utilisateurs

    // GET: Admin/Users
    public async Task<IActionResult> Users(
        string search = "",
        int page = 1,
        int pageSize = 10)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.Email.Contains(search) ||
                u.PhoneNumber.Contains(search));
        }

        var totalItems = await query.CountAsync();
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userVms = new List<UserViewModel>();
        foreach (var user in users)
        {
            userVms.Add(new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = await userManager.GetRolesAsync(user)
            });
        }

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.Search = search;
        ViewBag.AllRoles = await roleManager.Roles.ToListAsync();

        return View(userVms);
    }

    // GET: Admin/EditUserRoles/{userId}
    public async Task<IActionResult> EditUserRoles(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var vm = new UserRolesViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            CurrentRoles = await userManager.GetRolesAsync(user),
            AvailableRoles = await roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync()
        };

        return View(vm);
    }

    // POST: Admin/EditUserRoles
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserRoles(UserRolesViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = await userManager.FindByIdAsync(vm.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRolesAsync(user, vm.SelectedRoles);

        TempData["Success"] = "Rôles mis à jour avec succès";
        return RedirectToAction(nameof(Users));
    }

    // POST: Admin/DeleteUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return RedirectToAction(nameof(Users));
        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "Utilisateur supprimé avec succès";
        }
        else
        {
            TempData["Error"] = "Erreur lors de la suppression";
        }

        return RedirectToAction(nameof(Users));
    }

    #endregion

    #region Commandes

    // GET: Admin/Orders
    public async Task<IActionResult> Orders(
        OrderStatus? status = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status);
        }

        var totalItems = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.StatusList = Enum.GetValues<OrderStatus>();
        ViewBag.SelectedStatus = status;

        return View(orders);
    }

    // POST: Admin/UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null) return RedirectToAction(nameof(Orders));
        order.Status = status;
        await context.SaveChangesAsync();
        TempData["Success"] = "Statut de commande mis à jour";
        return RedirectToAction(nameof(Orders));
    }
    #endregion
}