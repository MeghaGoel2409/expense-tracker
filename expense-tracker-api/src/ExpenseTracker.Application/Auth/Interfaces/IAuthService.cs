using ExpenseTracker.Application.Auth.Commands;
using ExpenseTracker.Application.Auth.DTOs;
using ExpenseTracker.Application.Common.Models;

namespace ExpenseTracker.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<Result<AuthUserDto>> GetCurrentUserAsync(string userId, CancellationToken cancellationToken);
    Task<Result<AuthResultDto>> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
    Task<Result<AuthResultDto>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken);
    Task<Result<AuthResultDto>> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
    Task<Result> LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken);
}