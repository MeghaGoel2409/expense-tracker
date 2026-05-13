using ExpenseTracker.Application.Auth.Interfaces;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Features.Categories.Interfaces;
using ExpenseTracker.Application.Features.Dashboard.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Infrastructure.Authentication;
using ExpenseTracker.Infrastructure.Authentication.Interfaces;
using ExpenseTracker.Infrastructure.Identity;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExpenseTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =========================
        // Database
        // =========================
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // =========================
        // JWT Options (validated)
        // =========================
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("JwtSettings"))
            .ValidateDataAnnotations()
            .Validate(x => x.Secret.Length >= 32, "JwtSettings:Secret must be at least 32 characters.")
            .ValidateOnStart();

        // Resolve validated options
        var jwtOptions = configuration.GetSection("JwtSettings").Get<JwtOptions>()
            ?? throw new InvalidOperationException("JwtSettings not configured");

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Secret));

        // =========================
        // Authentication (JWT)
        // =========================
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // set true in production
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,

                IssuerSigningKey = signingKey,

                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        // =========================
        // Identity
        // =========================
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8; // improved

            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // =========================
        // Application Services
        // =========================
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        // =========================
        // Auth & Identity
        // =========================
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IAuthService, AuthService>();

        // Refresh Token Persistence
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Token Generators (stateless → singleton)
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

        // Current User
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // HttpContext accessor (needed for CurrentUserService)
        services.AddHttpContextAccessor();

        return services;
    }
}