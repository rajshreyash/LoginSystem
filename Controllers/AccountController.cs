using System.Security.Claims;
using LoginSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoginSystem.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Dashboard));
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model
    )
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string normalizedEmail = model.Email.Trim().ToLowerInvariant();

        ApplicationUser? existingUser =
            await _userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "An account with this email already exists."
            );

            return View(model);
        }

        var user = new ApplicationUser
        {
            Name = model.Name.Trim(),
            Email = normalizedEmail,
            UserName = normalizedEmail
        };

        IdentityResult result =
            await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] =
                "Registration successful. You can now log in.";

            return RedirectToAction(nameof(Login));
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Dashboard));
        }

        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null
    )
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string normalizedEmail = model.Email.Trim().ToLowerInvariant();

        ApplicationUser? user =
            await _userManager.FindByEmailAsync(normalizedEmail);

        if (user is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid email address or password."
            );

            return View(model);
        }

        Microsoft.AspNetCore.Identity.SignInResult result =
            await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid email address or password."
            );

            return View(model);
        }

        if (
            !string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl)
        )
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Dashboard));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        ApplicationUser? user =
            await _userManager.GetUserAsync(User);

        if (user is null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        ViewData["DisplayName"] = user.Name;
        ViewData["Email"] = user.Email;

        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}