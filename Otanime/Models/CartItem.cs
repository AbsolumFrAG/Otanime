using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Otanime.Models;

public class CartItem
{
    public int Id { get; init; }

    [Required]
    public int ProductId { get; init; }

    [ForeignKey("ProductId")]
    public Product Product { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être au moins 1")]
    public int Quantity { get; set; } = 1;

    [Required]
    public int CartId { get; init; }

    [ForeignKey("CartId")]
    public Cart Cart { get; init; }

    [Display(Name = "Date d'ajout")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [Display(Name = "Dernière modification")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Prix au moment de l'ajout au panier
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceSnapshot { get; init; }

    public void UpdateQuantity(int newQuantity)
    {
        if(newQuantity < 0) 
            throw new ArgumentException("La quantité ne peut pas être négative");

        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}