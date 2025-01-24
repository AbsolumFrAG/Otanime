using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Otanime.Models.ViewModels;
using Otanime.Services;

namespace Otanime.Controllers;

public class AccountController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    ICartService cartService)
    : Controller
{
    #region Inscription

    [HttpGet]
    public IActionResult Register()
    {
        if (signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
            await signInManager.SignInAsync(user, isPersistent: false);
            await cartService.MergeCartsAsync(user.Id);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
    #endregion

    #region Connexion
    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid) return View(model);
        var result = await signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            await cartService.MergeCartsAsync(user.Id);
            return RedirectToLocal(returnUrl);
        }
            
        ModelState.AddModelError(string.Empty, "Identifiants invalides");

        return View(model);
    }
    #endregion
    
    #region Déconnexion

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    #endregion
    
    #region Gestion du profil

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        return View(new ProfileViewModel
        {
            Email = user.Email,
            Username = user.UserName
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.GetUserAsync(User);
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                ViewData["SuccessMessage"] = "Profil mis à jour avec succès";
                return View("Profile", model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View("Profile", model);
    }
    #endregion
    
    #region Gestion des mots de passe

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await userManager.GetUserAsync(User);
        var result = await userManager.ChangePasswordAsync(
            user,
            model.OldPassword,
            model.NewPassword);

        if (result.Succeeded)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
    #endregion
    
    #region Panel Admin

    [Authorize(Roles = "Admin")]
    public IActionResult AdminPanel()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManageUsers()
    {
        var users = userManager.Users;
        var userRoles = new List<UserRolesViewModel>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userRoles.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            });
        }

        return View(userRoles);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ToggleAdminRole(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        if (await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.RemoveFromRoleAsync(user, "Admin");
        }
        else
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }

        return RedirectToAction("ManageUsers");
    }
    #endregion
    
    #region Méthodes privées

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
    #endregion
}