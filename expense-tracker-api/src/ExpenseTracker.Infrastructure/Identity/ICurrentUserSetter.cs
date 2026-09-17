using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Infrastructure.Identity
{
    public interface ICurrentUserSetter
    {
        void SetUser(string userId);
    }
}
