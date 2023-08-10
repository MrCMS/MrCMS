using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Models
{
    public class AclInfo : IEquatable<AclInfo>
    {
        public string Rule { get; set; }

        // split the rule on the dot and take the last part
        public string DisplayName => Rule.Split('.').LastOrDefault()?.BreakUpString() ?? Rule;
        public string Operation { get; set; }
        public string Key => AclKeyGenerator.GetKey(Rule, Operation);
        public List<AclRoleInfo> Roles { get; set; }
    
    
        public bool Equals(AclInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Rule == other.Rule && Operation == other.Operation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AclInfo) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rule, Operation);
        }

        public static bool operator ==(AclInfo left, AclInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AclInfo left, AclInfo right)
        {
            return !Equals(left, right);
        }


    }}