using ExpenseTracker.Application.Auth.DTOs;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<Result<AuthUserDto>> ValidateUserCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogWarning("Login failed because user was not found.");

            return Result<AuthUserDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!signInResult.Succeeded)
        {
            _logger.LogWarning("Login failed because password validation failed. UserId: {UserId}", user.Id);

            return Result<AuthUserDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Result<AuthUserDto>.Success(new AuthUserDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles
        });
    }

    public async Task<Result<AuthUserDto>> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("User lookup failed. UserId: {UserId}", userId);

            return Result<AuthUserDto>.Failure(
                Error.NotFound("Auth.UserNotFound", "User not found."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Result<AuthUserDto>.Success(new AuthUserDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles
        });
    }

    public async Task<Result<AuthUserDto>> RegisterUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            _logger.LogInformation("Registration failed because email is already registered. UserId: {UserId}", existingUser.Id);

            return Result<AuthUserDto>.Failure(
                Error.Conflict("Auth.EmailAlreadyExists", "A user with this email already exists."));
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            _logger.LogWarning("Identity user creation failed. ErrorCodes: {ErrorCodes}", createResult.Errors.Select(x => x.Code));

            var errors = createResult.Errors
                .Select(x => Error.Validation("Auth.RegisterFailed", x.Description))
                .ToArray();

            return Result<AuthUserDto>.Failure(errors);
        }

        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Identity user created successfully. UserId: {UserId}", user.Id);

        return Result<AuthUserDto>.Success(new AuthUserDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles
        });
    }

    public async Task<string> GetDefaultCurrencyAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        var currency = await _userManager.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.DefaultCurrency)
            .FirstOrDefaultAsync(cancellationToken);

        return string.IsNullOrWhiteSpace(currency)
            ? "USD"
            : currency;
    }
}
