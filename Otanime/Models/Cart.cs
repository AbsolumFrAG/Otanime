using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Otanime.Models;

public class Cart
{
    public int Id { get; init; }

    [StringLength(450)] // Correspond à la taille des IDs dans Identity
    public string? UserId { get; set; } // Nullable pour les invités

    [ForeignKey("UserId")]
    public IdentityUser? User { get; init; }

    [Required]
    [StringLength(36)] // Taille d'un GUID
    public string? SessionId { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [Display(Name = "Dernière activité")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Panier actif ?")]
    public bool IsActive { get; set; } = true;

    // Relations
    public List<CartItem> Items { get; init; } = [];

    // Méthodes utilitaires
    public decimal CalculateTotal() => Items.Sum(item => item.Quantity * item.PriceSnapshot);

    public void Merge(Cart otherCart)
    {
        foreach (var otherItem in otherCart.Items)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == otherItem.ProductId);
                
            if (existingItem != null)
            {
                existingItem.Quantity += otherItem.Quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = otherItem.ProductId,
                    Quantity = otherItem.Quantity,
                    PriceSnapshot = otherItem.PriceSnapshot
                });
            }
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearItems()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmpty => Items.Count == 0;
}