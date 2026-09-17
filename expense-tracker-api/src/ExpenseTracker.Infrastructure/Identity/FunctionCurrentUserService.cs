using ExpenseTracker.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Infrastructure.Identity
{
    public sealed class FunctionCurrentUserService :
     ICurrentUserService,
     ICurrentUserSetter
    {
        public string? UserId { get; private set; }

        public bool IsAuthenticated =>
            !string.IsNullOrWhiteSpace(UserId);

        public void SetUser(string userId)
        {
            UserId = userId;
        }
    }
}
