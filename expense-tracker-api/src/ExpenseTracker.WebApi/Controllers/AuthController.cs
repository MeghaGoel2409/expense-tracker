using ExpenseTracker.Application.Auth.Commands;
using ExpenseTracker.Application.Auth.DTOs;
using ExpenseTracker.Application.Auth.Interfaces;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.WebApi.Contracts.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

/// <summary>
/// Authentication endpoints for registration, login, token refresh, logout, and current user profile.
/// </summary>
[Tags("Authentication")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : BaseApiController
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<LoginUserRequest> _loginValidator;
    private readonly IValidator<RegisterUserRequest> _registerValidator;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService,
        IValidator<LoginUserRequest> loginValidator,
        IValidator<RegisterUserRequest> registerValidator)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    /// <summary>
    /// Registers a new user and returns an access token with user details.
    /// </summary>
    /// <remarks>
    /// Also sets the refresh token in an HttpOnly cookie.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<AuthTokenResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationFailure(validationResult.Errors);
        }

        var result = await _authService.RegisterAsync(
            new RegisterCommand
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                DeviceType = "browser"
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        var response = CreateAuthResponse(result.Value);
        return ToActionResult(Result<AuthTokenResponse>.Success(response), StatusCodes.Status201Created);
    }

    /// <summary>
    /// Logs in an existing user and returns an access token with user details.
    /// </summary>
    /// <remarks>
    /// Also sets the refresh token in an HttpOnly cookie.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationFailure(validationResult.Errors);
        }

        var result = await _authService.LoginAsync(
            new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                DeviceType = "browser"
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        var response = CreateAuthResponse(result.Value);
        return ToActionResult(Result<AuthTokenResponse>.Success(response));
    }

    /// <summary>
    /// Refreshes the access token using the refresh token cookie.
    /// </summary>
    /// <remarks>
    /// The refresh token is read from the HttpOnly cookie and rotated on success.
    /// </remarks>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(Result<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken = default)
    {
        var refreshToken = ResolveRefreshToken();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ToActionResult(
            Result<AuthTokenResponse>.Failure(
                Error.Unauthorized("Auth.RefreshTokenMissing", "Refresh token is missing.")));
        }

        var result = await _authService.RefreshAsync(
            new RefreshTokenCommand
            {
                RefreshToken = refreshToken,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            DeleteRefreshTokenCookie();
            return ToActionResult(result);
        }

        var response = CreateAuthResponse(result.Value);
        return ToActionResult(Result<AuthTokenResponse>.Success(response));
    }

    /// <summary>
    /// Logs out the current user and clears the refresh token cookie.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var refreshToken = ResolveRefreshToken();

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var result = await _authService.LogoutAsync(
                refreshToken,
                GetIpAddress(),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return ToActionResult(result);
            }
        }

        DeleteRefreshTokenCookie();
        return ToActionResult(Result.Success(), StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Returns the currently authenticated user.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(Result<AuthUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return ToActionResult(
                Result<AuthUserDto>.Failure(
                    Error.Unauthorized("Auth.UserIdMissing", "Authenticated user id is missing.")));
        }

        var result = await _authService.GetCurrentUserAsync(_currentUserService.UserId, cancellationToken);

        return ToActionResult(result);
    }

    private AuthTokenResponse CreateAuthResponse(AuthResultDto authResult)
    {
        AppendRefreshTokenCookie(authResult.RefreshToken, authResult.RefreshTokenExpiresAtUtc);

        return new AuthTokenResponse
        {
            AccessToken = authResult.AccessToken,
            ExpiresAtUtc = authResult.ExpiresAtUtc,
            User = authResult.User
        };
    }

    private string? ResolveRefreshToken()
    {
        return Request.Cookies[RefreshTokenCookieName];
    }

    private void AppendRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAtUtc,
                Path = "/", 
                IsEssential=true
            });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/auth/refresh"
            });
    }

    private string? GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return Request.Headers.UserAgent.ToString();
    }
}