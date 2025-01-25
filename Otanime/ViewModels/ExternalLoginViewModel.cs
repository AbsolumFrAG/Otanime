using System.ComponentModel.DataAnnotations;

namespace Otanime.ViewModels;

public class ExternalLoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}