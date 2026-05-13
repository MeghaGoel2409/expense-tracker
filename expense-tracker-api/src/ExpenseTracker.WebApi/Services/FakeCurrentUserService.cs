using ExpenseTracker.Application.Common.Interfaces;

namespace ExpenseTracker.WebApi.Services;

public class FakeCurrentUserService : ICurrentUserService
{
    public string? UserId => "test-user-1";

    public bool IsAuthenticated => true;
}