using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.Models;

namespace Otanime.Services;

public interface ICartService
{
    Task<Cart> GetCartAsync();
    Task AddToCartAsync(int productId, int quantity = 1);
    Task RemoveFromCartAsync(int productId, int quantity = 1);
    Task ClearCartAsync();
    Task MergeCartsAsync(string userId);
}

public class CartService(
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor)
    : ICartService
{
    public async Task<Cart> GetCartAsync()
    {
        var cart = await RetrieveCart();
        return cart ?? await CreateCart();
    }

    private async Task<Cart?> RetrieveCart()
    {
        var userId = GetUserId();
        var sessionId = GetSessionId();

        return await context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c =>
                (userId != null && c.UserId == userId) ||
                (sessionId != null && c.SessionId == sessionId));
    }

    private async Task<Cart> CreateCart()
    {
        var userId = GetUserId();
        var sessionId = GetSessionId() ?? Guid.NewGuid().ToString();

        if (userId != null)
        {
            var existingUserCart = await context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingUserCart != null) return existingUserCart;
        }

        var cart = new Cart
        {
            UserId = userId,
            SessionId = userId == null ? sessionId : null
        };

        await context.Carts.AddAsync(cart);
        await context.SaveChangesAsync();

        if (userId == null)
        {
            httpContextAccessor.HttpContext?.Session.SetString("SessionId", sessionId);
        }

        return cart;
    }

    public async Task AddToCartAsync(int productId, int quantity = 1)
    {
        var cart = await GetCartAsync();
        var product = await context.Products.FindAsync(productId);

        if (product == null || quantity < 1) return;

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                PriceSnapShot = product.Price
            });
        }

        await SaveCartAsync(cart);
    }

    public async Task RemoveFromCartAsync(int productId, int quantity = 1)
    {
        var cart = await GetCartAsync();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null) return;

        if (item.Quantity <= quantity)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity -= quantity;
        }

        await SaveCartAsync(cart);
    }

    public async Task ClearCartAsync()
    {
        var cart = await GetCartAsync();
        context.CartItems.RemoveRange(cart.Items);
        await context.SaveChangesAsync();
    }

    public async Task MergeCartsAsync(string userId)
    {
        var sessionId = GetSessionId();
        if (sessionId == null) return;

        var sessionCart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId);

        var userCart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (sessionCart == null) return;

        userCart ??= new Cart { UserId = userId };

        foreach (var sessionItem in sessionCart.Items)
        {
            var existingItem = userCart.Items
                .FirstOrDefault(i => i.ProductId == sessionItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += sessionItem.Quantity;
            }
            else
            {
                userCart.Items.Add(new CartItem
                {
                    ProductId = sessionItem.ProductId,
                    Quantity = sessionItem.Quantity,
                    PriceSnapShot = sessionItem.PriceSnapShot
                });
            }
        }

        if (userCart.Id == 0)
        {
            await context.Carts.AddAsync(userCart);
        }

        context.Carts.Remove(sessionCart);
        await context.SaveChangesAsync();
    }

    private async Task SaveCartAsync(Cart cart)
    {
        cart.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    private string? GetUserId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private string? GetSessionId()
    {
        return httpContextAccessor.HttpContext?.Session
            .GetString("SessionId");
    }
}