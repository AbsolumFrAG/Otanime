using System.ComponentModel.DataAnnotations;

namespace Otanime.Models.ViewModels;

public class CheckoutViewModel
{
    // Informations utilisateur
    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50, ErrorMessage = "Maximum 50 caractères")]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, ErrorMessage = "Maximum 50 caractères")]
    [Display(Name = "Nom")]
    public string LastName { get; set; } = string.Empty;

    // Adresse de livraison
    [Required(ErrorMessage = "L'adresse est obligatoire")]
    [Display(Name = "Adresse")]
    public string StreetAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ville est obligatoire")]
    [Display(Name = "Ville")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le code postal est obligatoire")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Code postal invalide")]
    [Display(Name = "Code postal")]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le pays est obligatoire")]
    [Display(Name = "Pays")]
    public string Country { get; set; } = "France";

    // Informations de paiement
    [Required(ErrorMessage = "Le numéro de carte est obligatoire")]
    [CreditCard(ErrorMessage = "Numéro de carte invalide")]
    [Display(Name = "Numéro de carte")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date d'expiration est obligatoire")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Format MM/YY")]
    [Display(Name = "Expiration")]
    public string CardExpiry { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le CVC est obligatoire")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Code de sécurité invalide")]
    [Display(Name = "CVC")]
    public string CardCvc { get; set; } = string.Empty;

    // Récapitulatif du panier
    public List<CartItemViewModel> CartItems { get; set; } = [];

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

    // Champs techniques
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
    public string? SessionId { get; set; }
    public int CartId { get; set; }

    [Required(ErrorMessage = "Vous devez accepter les conditions générales")]
    [Display(Name = "J'accepte les conditions générales de vente")]
    public bool TermsAccepted { get; set; }

    // Pour pré-remplissage si connecté
    public bool IsAuthenticated { get; set; }
}