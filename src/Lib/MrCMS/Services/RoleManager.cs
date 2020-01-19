using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class RoleManager : RoleManager<Role>, IRoleManager
    {
        public RoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger) : base(
            store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}