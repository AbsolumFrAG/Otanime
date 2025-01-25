using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.Models;

namespace Otanime.Services;

public interface ICartService
{
    Task<Cart?> GetCurrentCartAsync();
    Task AddItemToCartAsync(int productId, int quantity = 1);
    Task UpdateItemQuantityAsync(int itemId, int newQuantity);
    Task RemoveItemFromCartAsync(int itemId);
    Task ClearCartAsync();
    Task<int> GetCartItemCountAsync();
    Task MergeCartsOnLoginAsync(string userId);
}

public class CartService(
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    UserManager<IdentityUser> userManager)
    : ICartService
{
    public async Task<Cart?> GetCurrentCartAsync()
    {
        var (userId, sessionId) = GetUserIdentifiers();

        var cart = await FindExistingCartAsync(userId, sessionId) ?? await CreateNewCartAsync(userId, sessionId);

        return await LoadCartWithItemsAsync(cart.Id);
    }

    public async Task AddItemToCartAsync(int productId, int quantity = 1)
    {
        if (quantity < 1)
        {
            throw new ArgumentException("La quantité doit être supérieure à 0", nameof(quantity));
        }

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            throw new ArgumentException("Produit non trouvé", nameof(productId));
        }

        if (!product.InStock || product.StockQuantity < quantity)
        {
            throw new InvalidOperationException("Stock insuffisant");
        }

        var cart = await GetCurrentCartAsync();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            // Mise à jour de la quantité d'un article existant
            existingItem.Quantity += quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Ajout d'un nouvel article
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                PriceSnapshot = product.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task UpdateItemQuantityAsync(int itemId, int newQuantity)
    {
        if (newQuantity < 1)
        {
            throw new ArgumentException("La quantité doit être supérieure à 0", nameof(newQuantity));
        }

        var cartItem = await context.CartItems
            .Include(ci => ci.Product).Include(cartItem => cartItem.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == itemId);

        if (cartItem == null)
        {
            throw new ArgumentException("Article non trouvé", nameof(itemId));
        }

        if (!cartItem.Product.InStock || cartItem.Product.StockQuantity < newQuantity)
        {
            throw new InvalidOperationException("Stock insuffisant");
        }

        cartItem.Quantity = newQuantity;
        cartItem.UpdatedAt = DateTime.UtcNow;
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task RemoveItemFromCartAsync(int itemId)
    {
        var cartItem = await context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == itemId);

        if (cartItem == null)
        {
            return; // Item déjà supprimé ou n'existe pas
        }

        context.CartItems.Remove(cartItem);
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
    }

    public async Task ClearCartAsync()
    {
        var cart = await GetCurrentCartAsync();
        
        // Supprimer tous les articles du panier
        context.CartItems.RemoveRange(cart.Items);
        cart.UpdatedAt = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
    }

    public async Task<int> GetCartItemCountAsync()
    {
        var cart = await GetCurrentCartAsync();
        return cart.Items.Sum(i => i.Quantity);
    }

    public async Task MergeCartsOnLoginAsync(string userId)
    {
        // Trouver le panier utilisateur actif
        var userCart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        // Trouver le panier de session actif
        var sessionId = GetOrCreateSessionId();
        var sessionCart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);

        if (sessionCart == null)
        {
            return; // Pas de panier de session à fusionner
        }

        if (userCart == null)
        {
            // Si l'utilisateur n'a pas de panier, convertir le panier de session
            sessionCart.UserId = userId;
            sessionCart.SessionId = null;
            sessionCart.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Copier les articles du panier de session
            foreach (var sessionItem in sessionCart.Items)
            {
                var existingItem = userCart.Items
                    .FirstOrDefault(i => i.ProductId == sessionItem.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += sessionItem.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    userCart.Items.Add(new CartItem
                    {
                        CartId = userCart.Id,
                        ProductId = sessionItem.ProductId,
                        Quantity = sessionItem.Quantity,
                        PriceSnapshot = sessionItem.PriceSnapshot,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            // Marquer le panier de session comme inactif et sauvegarder
            sessionCart.IsActive = false;
            userCart.UpdatedAt = DateTime.UtcNow;

            // Supprimer le cookie de session
            if (httpContextAccessor.HttpContext != null)
            {
                httpContextAccessor.HttpContext.Response.Cookies.Delete("CartSessionId");
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task<Cart?> FindExistingCartAsync(string userId, string sessionId)
    {
        if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(sessionId))
        {
            return null;
        }

        var query = context.Carts
            .Include(c => c.Items)
            .Where(c => c.IsActive);

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(c => c.UserId == userId);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            query = query.Where(c => c.SessionId == sessionId);
        }

        return await query.FirstOrDefaultAsync();
    }

    private async Task<Cart> CreateNewCartAsync(string userId, string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
        }

        var newCart = new Cart
        {
            UserId = userId,
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            Items = []
        };

        context.Carts.Add(newCart);
        await context.SaveChangesAsync();
        
        return newCart;
    }

    private async Task<Cart?> LoadCartWithItemsAsync(int cartId)
    {
        var cart = await context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId && c.IsActive);

        if (cart == null)
        {
            throw new InvalidOperationException($"Le panier avec l'ID {cartId} n'a pas été trouvé ou n'est pas actif");
        }

        return cart;
    }

    private (string? userId, string sessionId) GetUserIdentifiers()
    {
        if (httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException("HttpContext n'est pas disponible");
        }

        var userId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
        var sessionId = GetOrCreateSessionId();

        return (userId, sessionId);
    }

    private string GetOrCreateSessionId()
    {
        if (httpContextAccessor?.HttpContext == null)
        {
            throw new InvalidOperationException("HttpContext n'est pas disponible");
        }

        var sessionId = httpContextAccessor.HttpContext.Request.Cookies["CartSessionId"];
        if (!string.IsNullOrEmpty(sessionId)) return sessionId;
        sessionId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMonths(1)
        };

        httpContextAccessor.HttpContext.Response.Cookies.Append("CartSessionId", sessionId, cookieOptions);

        return sessionId;
    }
}