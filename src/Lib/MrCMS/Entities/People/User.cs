using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Entities.People
{
    public class User : SystemEntity
    {
        public User()
        {
            Roles = new HashSet<UserRole>();
            UserProfileData = new List<UserProfileData>();
            UserLogins = new List<UserLogin>();
            UserClaims = new List<UserClaim>();
            UserTokens = new List<UserToken>();
        }

        public virtual IList<UserToken> UserTokens { get; set; }

        public virtual IList<UserClaim> UserClaims { get; set; }

        public virtual IList<UserLogin> UserLogins { get; set; }

        [DisplayName("First Name")] public virtual string FirstName { get; set; }

        [DisplayName("Last Name")] public virtual string LastName { get; set; }

        public virtual string Name =>
            string.IsNullOrWhiteSpace($"{FirstName} {LastName}")
                ? Email
                : $"{FirstName} {LastName}";

        public virtual byte[] PasswordHash { get; set; }
        public virtual byte[] PasswordSalt { get; set; }

        public virtual string CurrentEncryption { get; set; }

        [MaxLength(200)] public virtual string Source { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsUniqueEmail", "User", AdditionalFields = "Id")]
        public virtual string Email { get; set; }

        [Required] [DisplayName("Is Active")] public virtual bool IsActive { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }
        public virtual int LoginAttempts { get; set; }

        public virtual Guid? ResetPasswordGuid { get; set; }
        public virtual DateTime? ResetPasswordExpiry { get; set; }


        public virtual ISet<UserRole> Roles { get; set; }
        public virtual IList<UserProfileData> UserProfileData { get; set; }

        public virtual bool IsAdmin
        {
            get { return Roles != null && Roles.Any(role => role.Name == UserRole.Administrator); }
        }

        public static HashSet<Type> OwnedObjectTypes =>
            TypeHelper.GetAllConcreteMappedClassesAssignableFrom<IBelongToUser>();

        public virtual bool DisableNotifications { get; set; }
        public virtual DateTime? LastNotificationReadDate { get; set; }

        [DisplayName("Site UI Culture")] public virtual string UICulture { get; set; }

        public virtual string AvatarImage { get; set; }


        // Aspnet.Identity fields
        public virtual bool TwoFactorAuthEnabled { get; set; }
        public virtual string TwoFactorRecoveryCodes { get; set; }

        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string AuthenticatorKey { get; set; }
        public virtual DateTimeOffset? LockoutEndDate { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string SecurityStamp { get; set; }
    }
}