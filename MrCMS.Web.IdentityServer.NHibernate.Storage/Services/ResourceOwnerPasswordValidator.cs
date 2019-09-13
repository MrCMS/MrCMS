using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.People;
using MrCMS.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        protected readonly ILogger Logger;
        private readonly IUserLookup _userLookup;
        private readonly IPasswordManagementService _passwordManagementService;

        public ResourceOwnerPasswordValidator(ILogger<MrCMSIS4ProfileService> logger, IUserLookup userLookup, IPasswordManagementService passwordManagementService)
        {
            Logger = logger;
            _userLookup = userLookup;
            _passwordManagementService = passwordManagementService;
        }
        Task IResourceOwnerPasswordValidator.ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
           try
            {
                Logger.LogDebug("Validating the user Details");
                var user = _userLookup.GetUserByEmail(context.UserName);

                if (user != null && _passwordManagementService.ValidateUser(user, context.Password))
                {
                    Logger.LogDebug("Valid User Details");
                    context.Result = new GrantValidationResult(user.Id.ToString(), "custom");
                }
                else
                {
                    Logger.LogDebug("Invalid User Details");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            throw new NotImplementedException();
        }

        public static Claim[] GetUserClaims(User user)
        {
            var claimCollections =  new List<Claim>
            {
            new Claim("userid", user.Id.ToString() ?? ""),
             new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? user.Email),
                new Claim(ClaimTypes.Surname, user.LastName?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("username",  user.Email),

            //roles

           
        };

            foreach (var role in user.Roles)
            {
                claimCollections.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            return claimCollections.ToArray();
        }
    }
}
