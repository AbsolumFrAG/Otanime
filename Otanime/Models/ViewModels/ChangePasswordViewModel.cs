using System.ComponentModel.DataAnnotations;

namespace Otanime.Models.ViewModels;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe actuel")]
    public string OldPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nouveau mot de passe")]
    public string NewPassword { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string ConfirmPassword { get; set; }
}