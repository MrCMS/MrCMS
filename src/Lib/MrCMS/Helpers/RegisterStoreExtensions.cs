using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Helpers
{
    public static class RegisterStoreExtensions
    {
        public static IdentityBuilder AddMrCMSStores(this IdentityBuilder builder)
        {
            builder.Services.AddScoped<IUserStore<User>, UserStore>();
            builder.Services.AddScoped<IRoleStore<UserRole>, RoleStore>();
            return builder;
        }
    }
}