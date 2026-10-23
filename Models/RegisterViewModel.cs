using System.ComponentModel.DataAnnotations;
namespace LoginSystem.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Name must be between 2 and 100 characters."
    )]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage = "Password must contain at least 6 characters."
    )]

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Password and confirmation password do not match."
    )]
    public string ConfirmPassword { get; set; } = string.Empty;
}