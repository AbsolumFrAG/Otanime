﻿namespace Otanime.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
}