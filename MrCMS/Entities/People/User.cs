using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using Microsoft.AspNet.Identity;
using MrCMS.ACL;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Helpers.Validation;
using NHibernate;

namespace MrCMS.Entities.People
{
    public class UserLogin : SystemEntity
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual User User { get; set; }
    }
    public class UserClaim : SystemEntity
    {
        public virtual string Issuer { get; set; }
        public virtual string Claim { get; set; }
        public virtual string Value { get; set; }

        public virtual User User { get; set; }
    }
    public class User : SystemEntity, IUser
    {
        public User()
        {
            Guid = Guid.NewGuid();
            Roles = new HashedSet<UserRole>();
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
        public virtual string Name { get { return string.IsNullOrWhiteSpace(string.Format("{0} {1}", FirstName, LastName)) ? Email : string.Format("{0} {1}", FirstName, LastName); } }
        public virtual byte[] PasswordHash { get; set; }
        public virtual byte[] PasswordSalt { get; set; }

        public virtual Guid Guid { get; set; }

        public virtual string CurrentEncryption { get; set; }

        [Required]
        [EmailValidator]
        [Remote("IsUniqueEmail", "User", AdditionalFields = "Id")]
        public virtual string Email { get; set; }

        [Required]
        [DisplayName("Is Active")]
        public virtual bool IsActive { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }
        public virtual int LoginAttempts { get; set; }

        public virtual Guid? ResetPasswordGuid { get; set; }
        public virtual DateTime? ResetPasswordExpiry { get; set; }

        public virtual Iesi.Collections.Generic.ISet<UserRole> Roles { get; set; }
        protected internal virtual IList<UserProfileData> UserProfileData { get; set; }

        public virtual T Get<T>() where T : UserProfileData
        {
            return UserProfileData.OfType<T>().FirstOrDefault();
        }

        public virtual T2 Get<T1, T2>(Func<T1, T2> func) where T1 : UserProfileData
        {
            var firstOrDefault = UserProfileData.OfType<T1>().FirstOrDefault();
            return firstOrDefault == null ? default(T2) : func(firstOrDefault);
        }

        public virtual IEnumerable<T> GetAll<T>() where T : UserProfileData
        {
            return UserProfileData.OfType<T>();
        }

        public virtual bool IsAdmin
        {
            get { return Roles != null && Roles.Any(role => role.Name == UserRole.Administrator); }
        }

        public override void OnDeleting(ISession session)
        {
            base.OnDeleting(session);
            foreach (var userRole in Roles)
                userRole.Users.Remove(this);
            Roles.Clear();
        }

        public virtual bool CanAccess<T>(string operation, string type = null) where T : ACLRule, new()
        {
            return new T().CanAccess(this, operation, type);
        }

        public static List<Type> OwnedObjectTypes
        {
            get { return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<IBelongToUser>(); }
        }

        public virtual string OwinId
        {
            get { return Id.ToString(); }
        }
        string IUser.Id
        {
            get { return OwinId; }
        }

        string IUser.UserName
        {
            get { return Email; }
            set { Email = value; }
        }

        public virtual bool DisableNotifications { get; set; }
        public virtual DateTime? LastNotificationReadDate { get; set; }
    }
}