using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Otanime.Models;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public IdentityUser User { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 100000, ErrorMessage = "Montant total invalide")]
    public decimal TotalAmount { get; set; }
    
    [Required]
    [StringLength(200, MinimumLength = 10)]
    public string ShippingAddress { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [StringLength(50)]
    public string TrackingNumber { get; set; }
    
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; }
    
    public string PaymentIntentId { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}

public class OrderItem
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    
    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
}