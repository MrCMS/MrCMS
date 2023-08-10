using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        // try and get the value from the NameIdentifier claim first
        var value = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        // if that's not available, we're not logged in
        if (value == null)
        {
            return null;
        }

        // otherwise, try and parse the value as an int
        return int.TryParse(value, out var id) ? id : null;
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        return principal?.FindAll(ClaimTypes.Role).Select(x => x.Value) ?? Array.Empty<string>();
    }

    public static IEnumerable<int> GetRoleIds(this ClaimsPrincipal principal)
    {
        return principal?.FindAll(UserStore.RoleIdClaimType).Select(x => int.TryParse(x.Value, out var id) ? id : 0)
            .Where(x => x > 0).ToArray() ?? Array.Empty<int>();
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Email);
    }

    public static string GetFirstName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(ClaimTypes.GivenName);
    }

    public static string GetLastName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Surname);
    }

    public static string GetFullName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(ClaimTypes.Name);
    }

    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal?.IsInRole(role) ?? false;
    }

    public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.Any(principal.IsInRole);
    }
    
    public static bool IsInAnyRole(this ClaimsPrincipal principal, IEnumerable<string> roles)
    {
        return roles.Any(principal.IsInRole);
    }

    public static bool IsInAllRoles(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.All(principal.IsInRole);
    }
    
    public static bool IsInAllRoles(this ClaimsPrincipal principal, IEnumerable<string> roles)
    {
        return roles.All(principal.IsInRole);
    }

    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal?.IsInRole(UserRole.Administrator) ?? false;
    }

    public static bool DisableNotifications(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(UserStore.DisableNotificationsClaimType) == "true";
    }
    
    public static string GetAvatarUrl(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(UserStore.AvatarClaimType);
    }
    
    public static Guid? GetUserGuid(this ClaimsPrincipal principal)
    {
        var value = principal?.FindFirstValue(UserStore.UserGuidClaimType);

        return Guid.TryParse(value, out var guid) ? guid : null;
    }
    
    public static string GetUserCulture(this ClaimsPrincipal principal)
    {
        return principal?.FindFirstValue(UserStore.UserCultureClaimType);
    }
}