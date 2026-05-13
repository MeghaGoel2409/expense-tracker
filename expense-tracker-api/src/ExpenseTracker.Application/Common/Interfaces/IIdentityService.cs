using ExpenseTracker.Application.Auth.DTOs;
using ExpenseTracker.Application.Common.Models;

namespace ExpenseTracker.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<AuthUserDto>> ValidateUserCredentialsAsync(string email, 
        string password, CancellationToken cancellationToken);

    Task<Result<AuthUserDto>> GetUserByIdAsync(string userId, 
        CancellationToken cancellationToken);

    Task<Result<AuthUserDto>> RegisterUserAsync(string email, string password,
        string firstName, string lastName,
        CancellationToken cancellationToken);

    Task<string> GetDefaultCurrencyAsync(
    string userId,
    CancellationToken cancellationToken = default);
}