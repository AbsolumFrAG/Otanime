using System.ComponentModel.DataAnnotations;

namespace Otanime.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Se souvenir de moi")]
    public bool RememberMe { get; set; }
}