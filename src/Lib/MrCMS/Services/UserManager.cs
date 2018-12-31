using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class UserManager : UserManager<User>, IUserPasswordManager, IUserClaimManager, IUserLoginManager,
        IUserRoleManager, IUserEmailManager, IUserPhoneNumberManager, IUserLockoutManager,
        IUser2FAManager, IUserTokenManager, IGetUserFromClaims//, IUserLookup //IUserManager
    {
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store,
            optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services,
            logger)
        {
        }

        public async Task<IdentityResult> AssignRoles(User user, IList<string> roles)
        {
            var existingRoles = await GetRolesAsync(user);
            var toAdd = roles.Except(existingRoles, StringComparer.OrdinalIgnoreCase);
            var toRemove = existingRoles.Except(roles, StringComparer.OrdinalIgnoreCase);

            var result = await AddToRolesAsync(user, toAdd);
            if (!result.Succeeded)
                return result;

            result = await RemoveFromRolesAsync(user, toRemove);
            if (!result.Succeeded)
                return result;

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> SetPassword(User user, string password)
        {
            var result = await RemovePasswordAsync(user);
            if (!result.Succeeded)
                return result;
            return await AddPasswordAsync(user, password);
        }
    }
}