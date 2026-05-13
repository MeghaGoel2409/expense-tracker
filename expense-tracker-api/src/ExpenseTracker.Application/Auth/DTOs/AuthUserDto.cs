using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Auth.DTOs
{
    public sealed class AuthUserDto
    {
        public string UserId { get; init; } = default!;
        public string Email { get; init; } = default!;

        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string FullName => $"{FirstName} {LastName}".Trim();

        public IList<string> Roles { get; init; } = Array.Empty<string>();
    }
}
