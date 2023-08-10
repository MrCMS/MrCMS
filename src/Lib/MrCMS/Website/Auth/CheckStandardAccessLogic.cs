﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MrCMS.ACL;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth
{
    public class CheckStandardAccessLogic : ICheckStandardAccessLogic
    {
        public StandardLogicCheckResult Check(ClaimsPrincipal user)
        {
            // must be logged in
            if (user == null)
                return new StandardLogicCheckResult {CanAccess = false};

            return GetResult(user);
        }

        private StandardLogicCheckResult GetResult(ClaimsPrincipal user)
        {
            // if they're an admin they're always allowed
            if (user.IsAdmin())
                return new StandardLogicCheckResult {CanAccess = true};

            // if the user has no roles, they cannot have any acl access granted
            var roles = user.GetRoleIds();
            if (!roles.Any())
                return new StandardLogicCheckResult {CanAccess = false};

            return new StandardLogicCheckResult {Roles = roles.ToHashSet()};
        }
    }}