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
            UserToRoles = new List<UserToRole>();
            UserProfileData = new List<UserProfileData>();
            UserLogins = new List<UserLogin>();
            UserClaims = new List<UserClaim>();
        }

        public virtual IList<UserClaim> UserClaims { get; set; }

        public virtual IList<UserLogin> UserLogins { get; set; }

        [DisplayName("First Name")]
        public virtual string FirstName { get; set; }

        [DisplayName("Last Name")]
        public virtual string LastName { get; set; }

        public virtual string Name =>
            string.IsNullOrWhiteSpace($"{FirstName} {LastName}")
                ? Email
                : $"{FirstName} {LastName}";

        public virtual byte[] PasswordHash { get; set; }
        public virtual byte[] PasswordSalt { get; set; }

        public virtual string CurrentEncryption { get; set; }

        [MaxLength(200)]
        public virtual string Source { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsUniqueEmail", "User", AdditionalFields = "Id")]
        public virtual string Email { get; set; }

        [Required]
        [DisplayName("Is Active")]
        public virtual bool IsActive { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }
        public virtual int LoginAttempts { get; set; }

        public virtual Guid? ResetPasswordGuid { get; set; }
        public virtual DateTime? ResetPasswordExpiry { get; set; }

        public virtual string TwoFactorCode { get; set; }
        public virtual DateTime? TwoFactorCodeExpiry { get; set; }

        public virtual IList<UserToRole> UserToRoles { get; set; }
        public virtual IList<UserProfileData> UserProfileData { get; set; }

        public virtual bool IsAdmin
        {
            get { return UserToRoles != null && UserToRoles.Any(role => role.UserRole.Name == Role.Administrator); }
        }

        public static HashSet<Type> OwnedObjectTypes
        {
            get { return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<IBelongToUser>(); }
        }

        public virtual bool DisableNotifications { get; set; }
        public virtual DateTime? LastNotificationReadDate { get; set; }

        [DisplayName("Site UI Culture")]
        public virtual string UICulture { get; set; }

        public virtual string AvatarImage { get; set; }
    }
}