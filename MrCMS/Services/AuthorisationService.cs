using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using System.Linq;

namespace MrCMS.Services
{
    public class AuthorisationService : IAuthorisationService
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly UserManager<User> _userManager;

        public AuthorisationService(IAuthenticationManager authenticationManager, UserManager<User> userManager)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        public void SetAuthCookie(User user, bool rememberMe)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }

        public void Logout()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        public void UpdateClaims(User user, IEnumerable<Claim> claims)
        {
            var existingClaims = _userManager.GetClaims(user.OwinId).ToList();
            var list = claims as IList<Claim> ?? claims.ToList();
            var newClaims =
                list.Where(claim => existingClaims.All(c => c.Type != claim.Type));
            var updatedClaims =
                list.Where(claim => existingClaims.Any(c => c.Type == claim.Type && c.Value != claim.Value));
            foreach (var newClaim in newClaims)
                _userManager.AddClaim(user.OwinId, newClaim);
            foreach (var updatedClaim in updatedClaims)
            {
                var existing = existingClaims.First(c => c.Type == updatedClaim.Type);
                _userManager.RemoveClaim(user.OwinId, existing);
                _userManager.AddClaim(user.OwinId, updatedClaim);
            }
        }
    }

    public struct ClaimsComparison : IEquatable<ClaimsComparison>
    {
        public ClaimsComparison(Claim claim) : this()
        {
            Issuer = claim.Issuer;
            Type = claim.Type;
            Value = claim.Value;
        }

        public string Issuer { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public bool Equals(ClaimsComparison other)
        {
            return base.Equals(other);
        }
    }
}