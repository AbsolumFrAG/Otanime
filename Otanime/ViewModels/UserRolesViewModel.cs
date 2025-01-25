namespace Otanime.ViewModels;

public class UserRolesViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public List<string> AvailableRoles { get; set; }
    public List<string> SelectedRoles { get; set; }
    public IList<string> CurrentRoles { get; set; }
}