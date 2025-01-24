using System.ComponentModel.DataAnnotations;

namespace Otanime.Models.ViewModels;

public class CartViewModel
{
    public int CartId { get; set; }

    [Display(Name = "Articles")]
    private List<CartItemViewModel> Items { get; set; } = [];

    [Display(Name = "Sous-total")]
    [DataType(DataType.Currency)]
    public decimal Subtotal { get; set; }

    [Display(Name = "Frais de livraison")]
    [DataType(DataType.Currency)]
    private decimal ShippingCost { get; set; } = 4.99m;

    [Display(Name = "TVA (20%)")]
    [DataType(DataType.Currency)]
    private decimal TaxAmount => (Subtotal + ShippingCost) * 0.20m;

    [Display(Name = "Total")]
    [DataType(DataType.Currency)]
    public decimal GrandTotal => Subtotal + ShippingCost + TaxAmount;

    [Display(Name = "Nombre d'articles")]
    public int TotalItems => Items.Sum(i => i.Quantity);
    
    public bool IsLoggedIn { get; set; }
    public string? SessionId { get; set; }
    
    public List<string> ValidationMessages { get; set; } = [];
}

public abstract class CartItemViewModel
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }

    [Display(Name = "Nom")]
    public string ProductName { get; set; } = string.Empty;
    
    [Display(Name = "Prix unitaire")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    
    [Display(Name = "Quantité")]
    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Display(Name = "Total")]
    [DataType(DataType.Currency)]
    public decimal LineTotal => Price * Quantity;
    
    [Display(Name = "Image")]
    public string ImageUrl { get; set; } = string.Empty;
    
    [Display(Name = "Stock disponible")]
    public int StockQuantity { get; set; }
}