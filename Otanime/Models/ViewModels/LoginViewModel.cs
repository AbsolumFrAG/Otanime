using System.ComponentModel.DataAnnotations;

namespace Otanime.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Le mot de passe est requis")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "Se souvenir de moi")]
    public bool RememberMe { get; set; }
}