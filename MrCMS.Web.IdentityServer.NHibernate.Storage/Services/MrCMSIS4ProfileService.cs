using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using MrCMS.Services;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public class MrCMSIS4ProfileService : IdentityServer4.Services.IProfileService
    {
        protected readonly ILogger Logger;
        private readonly IUserLookup _userLookup;

        public MrCMSIS4ProfileService(ILogger<MrCMSIS4ProfileService> logger, IUserLookup userLookup)
        {
            Logger = logger;
            _userLookup = userLookup;
        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var sub = context.Subject.GetSubjectId();

                Logger.LogInformation("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                    context.Subject.GetSubjectId(),
                    context.Client.ClientName ?? context.Client.ClientId,
                    context.RequestedClaimTypes,
                    context.Caller);
                //  var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");
                var subjectId = context.Subject.GetSubjectId();
                if (!string.IsNullOrEmpty(subjectId) && int.Parse(subjectId) > 0)
                {
                    var userId = int.Parse(subjectId);
                    var user = _userLookup.GetUserById(userId);
                    var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? user.Email),
                new Claim(ClaimTypes.Surname, user.LastName?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("username",  user.Email),
               // new Claim("phonenumber", user.TwoFactorDetails()?.PhoneNumber ?? "")

            };

                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }

                    context.IssuedClaims = claims;
                    return Task.FromResult(0);

                }
                else
                {
                    Logger.LogDebug("Get second profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                  context.Subject.Identity.Name,
                  context.Client.ClientName ?? context.Client.ClientId,
                  context.RequestedClaimTypes,
                  context.Caller);
                    if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                    {
                        //get user from db (in my case this is by email)
                        var user = _userLookup.GetUserByEmail(context.Subject.Identity.Name);

                        if (user != null)
                        {
                            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? user.Email),
                new Claim(ClaimTypes.Surname, user.LastName?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("username",  user.Email),
               // new Claim("phonenumber", user.TwoFactorDetails()?.PhoneNumber ?? "")

            };

                            foreach (var role in user.Roles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role.Name));
                            }

                            context.IssuedClaims = claims;
                            return Task.FromResult(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(0);
            //  throw new NotImplementedException();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
              
               //get subject from context (set in ResourceOwnerPasswordValidator.ValidateAsync),
               var userEmail = context.Subject.Claims.FirstOrDefault(x => x.Type == "email");
                Logger.LogDebug("Checking if User {username} is Active", userEmail.Value);
                if (!string.IsNullOrEmpty(userEmail?.Value))
                {
                    var user = _userLookup.GetUserByEmail(userEmail.Value);

                    if (user != null)
                    {
                        if (user.IsActive)
                        {
                            context.IsActive = user.IsActive;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //handle error logging
            }

            return Task.FromResult(0);
        }
    }
}
