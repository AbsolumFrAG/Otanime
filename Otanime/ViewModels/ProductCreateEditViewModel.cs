using System.ComponentModel.DataAnnotations;

namespace Otanime.ViewModels;

public class ProductCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    [Url]
    [Display(Name = "URL de l'image")]
    public string ImageUrl { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }

    [Display(Name = "En stock")]
    public bool InStock { get; set; } = true;

    [Display(Name = "Quantité en stock")]
    [Range(0, 1000)]
    public int StockQuantity { get; set; } = 100;
}