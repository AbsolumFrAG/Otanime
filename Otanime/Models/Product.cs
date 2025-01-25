using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Otanime.Models;

public class Product
{
    public int Id { get; init; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Le nom doit contenir entre 3 et 100 caractères")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Le prix est obligatoire")]
    [Range(0.01, 10000, ErrorMessage = "Le prix doit être compris entre 0.01 et 10 000")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "La description est obligatoire")]
    [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
    public string Description { get; set; }

    [Required(ErrorMessage = "L'URL de l'image est obligatoire")]
    [Url(ErrorMessage = "Veuillez entrer une URL valide")]
    [Display(Name = "URL de l'image")]
    public string ImageUrl { get; set; }

    [Display(Name = "Date de création")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "En stock")]
    public bool InStock { get; set; } = true;

    [Range(0, 1000, ErrorMessage = "Le stock doit être entre 0 et 1000")]
    public int StockQuantity { get; set; } = 100;

    // Catégorie optionnelle
    [StringLength(50, ErrorMessage = "La catégorie ne peut pas dépasser 50 caractères")]
    public string? Category { get; set; }

    // Relations
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}