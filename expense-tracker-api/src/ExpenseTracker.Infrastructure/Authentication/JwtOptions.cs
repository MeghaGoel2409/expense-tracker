using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Infrastructure.Authentication;

public sealed class JwtOptions
{
    [Required]
    [MinLength(32, ErrorMessage ="{0} must be atleast 32 characters.")]
    public string Secret { get; set; } = default!;

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    [Range(1, 1440)]
    public int AccessTokenExpiryMinutes { get; set; }

    [Range(1,365)]
    public int RefreshTokenExpiryDays { get; set; }
}