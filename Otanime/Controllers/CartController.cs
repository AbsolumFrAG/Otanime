using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.Models;
using Otanime.Services;
using Otanime.ViewModels;

namespace Otanime.Controllers;

public class CartController(
    ICartService cartService,
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var cart = await cartService.GetCurrentCartAsync();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        if (quantity < 1)
        {
            TempData["ErrorMessage"] = "Quantité invalide";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        try
        {
            await cartService.AddItemToCartAsync(productId, quantity);
            TempData["SuccessMessage"] = "Produit ajouté au panier";
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de l'ajout au panier";
        }

        return RedirectToAction("Details", "Product", new { id = productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int itemId, int newQuantity)
    {
        if (newQuantity < 1)
        {
            return BadRequest("Quantité invalide");
        }

        try
        {
            await cartService.UpdateItemQuantityAsync(itemId, newQuantity);

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                var cart = await cartService.GetCurrentCartAsync();
                return Json(new
                {
                    success = true,
                    total = cart.CalculateTotal().ToString("C"),
                    itemCount = cart.Items.Sum(i => i.Quantity)
                });
            }

            TempData["SuccessMessage"] = "Quantité mise à jour";
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur de mise à jour";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        try
        {
            await cartService.RemoveItemFromCartAsync(itemId);
            TempData["SuccessMessage"] = "Article retiré du panier";
        }
        catch
        {
            TempData["ErrorMessage"] = "Erreur lors de la suppression";
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = await cartService.GetCurrentCartAsync();
        if (cart is { Items.Count: 0 })
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await userManager.GetUserAsync(User);
        var vm = new CheckoutViewModel
        {
            Total = cart.CalculateTotal(),
            ShippingAddress = user?.Email
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var cart = await cartService.GetCurrentCartAsync();
        if (cart.Items.Count == 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await userManager.GetUserAsync(User);
        
        var paymentReference = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        var order = new Order
        {
            UserId = user.Id,
            Total = cart.CalculateTotal(),
            ShippingAddress = model.ShippingAddress,
            City = model.City,
            PostalCode = model.PostalCode,
            Country = model.Country,
            PhoneNumber = model.PhoneNumber,
            Email = user.Email,
            PaymentMethod = model.PaymentMethod,
            PaymentReference = paymentReference,
            OrderItems = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.PriceSnapshot
            }).ToList()
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        await cartService.ClearCartAsync();

        return RedirectToAction("OrderConfirmation", new { id = order.Id });
    }

    [Authorize]
    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order?.UserId != userManager.GetUserId(User))
        {
            return NotFound();
        }

        return View(order);
    }
}