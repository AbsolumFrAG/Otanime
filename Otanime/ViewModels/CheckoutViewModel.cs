using System.ComponentModel.DataAnnotations;

namespace Otanime.ViewModels;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Adresse requise")]
    public string? ShippingAddress { get; set; }

    [Required(ErrorMessage = "Ville requise")]
    public string City { get; set; }

    [Required(ErrorMessage = "Code postal requis")]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "Pays requis")]
    public string Country { get; set; }

    [Phone(ErrorMessage = "Numéro invalide")]
    public string PhoneNumber { get; set; }

    [Required]
    public decimal Total { get; set; }
    
    [Required(ErrorMessage = "Le mode de paiement est requis")]
    [Display(Name = "Mode de paiement")]
    public string PaymentMethod { get; set; }
}