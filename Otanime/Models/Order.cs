using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Otanime.Models;

public class Order
{
    public int Id { get; init; }

    [Required]
    public string UserId { get; init; }

    [ForeignKey("UserId")]
    public IdentityUser User { get; init; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 100000, ErrorMessage = "Le total doit être supérieur à 0")]
    public decimal Total { get; init; }

    [Required]
    public DateTime OrderDate { get; init; } = DateTime.UtcNow;

    [Required]
    [StringLength(255)]
    public string ShippingAddress { get; init; }

    [Required]
    [StringLength(100)]
    public string City { get; init; }

    [Required]
    [StringLength(20)]
    public string PostalCode { get; init; }

    [Required]
    [StringLength(50)]
    public string Country { get; init; }

    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; init; }

    [EmailAddress]
    [StringLength(100)]
    public string Email { get; init; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Relations
    public ICollection<OrderItem> OrderItems { get; init; } = new List<OrderItem>();

    // Payment information
    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; init; }

    [Required]
    [StringLength(100)]
    public string PaymentReference { get; init; }

    public DateTime? PaymentDate { get; init; }
}

public class OrderItem
{
    public int Id { get; init; }

    [Required]
    public int ProductId { get; init; }

    [ForeignKey("ProductId")]
    public Product Product { get; init; }

    [Required]
    public int Quantity { get; init; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; init; }

    [Required]
    public int OrderId { get; init; }

    [ForeignKey("OrderId")]
    public Order Order { get; init; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}
