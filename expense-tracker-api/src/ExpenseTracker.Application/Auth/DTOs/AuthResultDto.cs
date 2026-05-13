using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Auth.DTOs
{
    public sealed class AuthResultDto
    {
        public string AccessToken { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }

        public string RefreshToken { get; init; } = default!;
        public DateTime RefreshTokenExpiresAtUtc { get; init; }
        public AuthUserDto User { get; init; } = default!;
    }
}
