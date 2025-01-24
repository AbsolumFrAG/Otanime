using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Otanime.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Le nom doit contenir entre 3 et 100 caractères")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La description est obligatoire")]
    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Le prix est obligatoire")]
    [Range(0.01, 10000, ErrorMessage = "Le prix doit être entre 0.01€ et 10 000€")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "L'URL de l'image est obligatoire")]
    [Url(ErrorMessage = "URL invalide")]
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La catégorie est obligatoire")]
    public FigureCategory Category { get; set; }
    
    [Range(1, 1000, ErrorMessage = "La taille doit être dans 1cm et 1000cm")]
    public int HeightCm { get; set; }
    
    [Required(ErrorMessage = "Le fabricant est obligatoire")]
    [StringLength(50)]
    public string Manufacturer { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
    
    [Range(0, 10000, ErrorMessage = "Le stock ne peut pas être négatif")]
    public int StockQuantity { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public virtual ICollection<Product> RelatedProducts { get; set; } = new List<Product>();

    public enum FigureCategory
    {
        Figurine,
        Nendoroid,
        ModelKit,
        ScaleFigure,
        PrizeFigure,
        Figma,
        Other
    }
}