using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class RoleManager : RoleManager<UserRole>, IRoleManager
    {
        public RoleManager(IRoleStore<UserRole> store, IEnumerable<IRoleValidator<UserRole>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<UserRole>> logger) : base(
            store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}