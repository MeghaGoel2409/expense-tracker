using ExpenseTracker.Application.Auth.Commands;
using ExpenseTracker.Application.Auth.DTOs;
using ExpenseTracker.Application.Auth.Interfaces;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Infrastructure.Authentication.Interfaces;
using ExpenseTracker.Infrastructure.Persistence.Entities;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Infrastructure.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IOptions<JwtOptions> jwtOptions,
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task<Result<AuthResultDto>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var registerResult = await _identityService.RegisterUserAsync(
            command.Email,
            command.Password,
            command.FirstName,
            command.LastName,
            cancellationToken);

        if (!registerResult.IsSuccess)
        {
            _logger.LogInformation("User registration failed.");
            return registerResult.Errors;
        }

        var user = registerResult.Value;

        _logger.LogInformation("User registered successfully. UserId: {UserId}", user.UserId);

        return await IssueTokensAsync(
            user,
            command.IpAddress,
            command.UserAgent,
            command.DeviceType,
            cancellationToken);
    }

    public async Task<Result<AuthResultDto>> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _identityService.ValidateUserCredentialsAsync(
            command.Email,
            command.Password,
            cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login failed.");
            return result.Errors;
        }

        var user = result.Value;

        _logger.LogInformation("Login successful. UserId: {UserId}", user.UserId);

        return await IssueTokensAsync(
            user,
            command.IpAddress,
            command.UserAgent,
            command.DeviceType,
            cancellationToken);
    }

    public async Task<Result<AuthResultDto>> RefreshAsync(
    RefreshTokenCommand command,
    CancellationToken cancellationToken)
    {
        var incomingTokenHash = _refreshTokenGenerator.ComputeHash(command.RefreshToken);

        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(
            incomingTokenHash,
            cancellationToken);

        if (existingToken is null)
        {
            _logger.LogWarning("Refresh token validation failed. Token was not found.");

            return Result<AuthResultDto>.Failure(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid refresh token."));
        }

        var now = DateTime.UtcNow;

        // Token reuse detection: token was already rotated/replaced
        if (existingToken.ReplacedByTokenHash is not null)
        {
            _logger.LogWarning("Refresh token reuse detected. Revoking all active refresh tokens. UserId: {UserId}", existingToken.UserId);
            await _refreshTokenRepository.RevokeAllActiveTokensAsync(
                existingToken.UserId,
                command.IpAddress,
                cancellationToken);

            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return Result<AuthResultDto>.Failure(
                Error.Unauthorized(
                    "Auth.RefreshTokenReuseDetected",
                    "Refresh token is no longer valid. Please login again."));
        }

        // Token expired or manually revoked
        if (!existingToken.IsActive)
        {
            _logger.LogInformation("Inactive or expired refresh token used. UserId: {UserId}", existingToken.UserId);

            return Result<AuthResultDto>.Failure(
                Error.Unauthorized(
                    "Auth.RefreshTokenExpired",
                    "Refresh token has expired. Please login again."));
        }

        var userResult = await _identityService.GetUserByIdAsync(
            existingToken.UserId,
            cancellationToken);

        if (!userResult.IsSuccess)
        {
            _logger.LogWarning("Refresh token belongs to a user that could not be loaded. UserId: {UserId}", existingToken.UserId);
            return userResult.Errors;
        }

        var newRefreshToken = _refreshTokenGenerator.GenerateToken();
        var newRefreshTokenHash = _refreshTokenGenerator.ComputeHash(newRefreshToken);

        existingToken.Revoke(
            now,
            command.IpAddress,
            newRefreshTokenHash);

        var replacementToken = new RefreshToken(
            existingToken.UserId,
            newRefreshTokenHash,
            now.AddDays(_jwtOptions.RefreshTokenExpiryDays),
            now,
            command.IpAddress,
            existingToken.DeviceType,
            command.UserAgent);

        await _refreshTokenRepository.AddAsync(replacementToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated successfully. UserId: {UserId}", existingToken.UserId);

        var user = userResult.Value;

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            user.UserId,
            user.Email,
            user.Roles);

        return new AuthResultDto
        {
            AccessToken = accessToken,
            ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAtUtc = replacementToken.ExpiresAtUtc,
            User = user
        };
    }

    public async Task<Result> LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogDebug("Logout requested without refresh token.");
            return Result.Success();
        }

        var tokenHash = _refreshTokenGenerator.ComputeHash(refreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            _logger.LogDebug("Logout requested with missing or inactive refresh token.");
            return Result.Success();
        }

        existingToken.Revoke(DateTime.UtcNow, ipAddress);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged out. UserId: {UserId}", existingToken.UserId);

        return Result.Success();
    }

    private async Task<Result<AuthResultDto>> IssueTokensAsync(
        AuthUserDto user,
        string? ipAddress,
        string? userAgent,
        string? deviceType,
        CancellationToken cancellationToken)
    {
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.UserId, user.Email, user.Roles);

        var refreshToken = _refreshTokenGenerator.GenerateToken();
        var refreshTokenHash = _refreshTokenGenerator.ComputeHash(refreshToken);
        DateTime now = DateTime.UtcNow;

        var refreshTokenEntity = new RefreshToken(
            user.UserId,
            refreshTokenHash,
            now.Add(TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiryDays)),
            now,
            ipAddress,
            deviceType,
            userAgent);

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Refresh token issued. UserId: {UserId}", user.UserId);

        return new AuthResultDto
        {
            AccessToken = accessToken,
            ExpiresAtUtc = now.Add(TimeSpan.FromMinutes(_jwtOptions.AccessTokenExpiryMinutes)),
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenEntity.ExpiresAtUtc,
            User = user
        };
    }

    public async Task<Result<AuthUserDto>> GetCurrentUserAsync(string userId, CancellationToken cancellationToken)
    {
        var userResult = await _identityService.GetUserByIdAsync(userId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            _logger.LogWarning("Current user lookup failed. UserId: {UserId}", userId);
            return userResult.Errors;
        }

        return userResult.Value;
    }
}