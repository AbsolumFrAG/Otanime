using System.ComponentModel.DataAnnotations;

namespace Otanime.Models.ViewModels;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Display(Name = "Nom d'utilisateur")]
    public string Username { get; set; }
}