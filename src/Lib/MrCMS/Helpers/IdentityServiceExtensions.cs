using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;

namespace MrCMS.Helpers
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddMrCMSIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, UserRole>(options =>
            {
                // lockout
                if (int.TryParse(configuration["Auth:Lockout:MaxFailedAccessAttempts"],
                    out var maxFailedAccessAttempts))
                {
                    options.Lockout.MaxFailedAccessAttempts = maxFailedAccessAttempts;
                }

                if (TimeSpan.TryParse(configuration["Auth:Lockout:DefaultLockoutTimeSpan"],
                    out var defaultLockoutTimeSpan))
                {
                    options.Lockout.DefaultLockoutTimeSpan = defaultLockoutTimeSpan;
                }

                if (bool.TryParse(configuration["Auth:Lockout:AllowedForNewUsers"], out var allowedForNewUsers))
                {
                    options.Lockout.AllowedForNewUsers = allowedForNewUsers;
                }

                // password
                if (int.TryParse(configuration["Auth:Password:RequiredLength"], out var requiredLength))
                {
                    options.Password.RequiredLength = requiredLength;
                }

                if (bool.TryParse(configuration["Auth:Password:RequireDigit"], out var requireDigit))
                {
                    options.Password.RequireDigit = requireDigit;
                }

                if (bool.TryParse(configuration["Auth:Password:RequireLowercase"], out var requireLowercase))
                {
                    options.Password.RequireLowercase = requireLowercase;
                }

                if (bool.TryParse(configuration["Auth:Password:RequireNonAlphanumeric"],
                    out var requireNonAlphanumeric))
                {
                    options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;
                }

                if (bool.TryParse(configuration["Auth:Password:RequireUppercase"], out var requireUppercase))
                {
                    options.Password.RequireUppercase = requireUppercase;
                }

                // sign in
                if (bool.TryParse(configuration["Auth:SignIn:RequireConfirmedEmail"],
                    out var requireConfirmedEmail))
                {
                    options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
                }

                if (bool.TryParse(configuration["Auth:SignIn:RequireConfirmedPhoneNumber"],
                    out var requireConfirmedPhoneNumber))
                {
                    options.SignIn.RequireConfirmedPhoneNumber = requireConfirmedPhoneNumber;
                }

                options.User.RequireUniqueEmail = true;
                options.ClaimsIdentity.UserNameClaimType = nameof(User.Email);
                options.ClaimsIdentity.UserIdClaimType = nameof(User.Id);
            })
              .AddMrCMSStores()
              .AddDefaultTokenProviders();

            return services;
        }
    }
}