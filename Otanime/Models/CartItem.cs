using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Otanime.Models;

public class CartItem
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    
    [Required]
    [Range(1, 1000, ErrorMessage = "la quantité doit être entre 1 et 1000")]
    public int Quantity { get; set; }
    
    [Required]
    public int CartId { get; set; }
    
    [ForeignKey("CartId")]
    public Cart Cart { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceSnapShot { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}