using System.ComponentModel.DataAnnotations;

namespace Otanime.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Le mot de passe est requis")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string ConfirmPassword { get; set; }
}