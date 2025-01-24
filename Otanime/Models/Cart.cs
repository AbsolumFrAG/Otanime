using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Otanime.Models;

public class Cart
{
    public int Id { get; set; }
    
    [StringLength(450)]
    public string? UserId { get; set; }
    
    [StringLength(100)]
    public string? SessionId { get; set; }

    public List<CartItem> Items { get; set; } = [];

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; }

    [NotMapped] public decimal TotalPrice => Items.Sum(item => item.Product.Price * item.Quantity);

    public void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public CartItem? FindItem(int productId)
    {
        return Items.FirstOrDefault(i => i.ProductId == productId);
    }
    
    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }
}