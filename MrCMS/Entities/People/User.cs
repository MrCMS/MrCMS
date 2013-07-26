using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Entities.People
{
    public class User : SystemEntity
    {
        public User()
        {
            Guid = Guid.NewGuid();
            Roles = new List<UserRole>();
            Sites = new List<Site>();
            UserProfileData = new List<UserProfileData>();
        }

        [DisplayName("First Name")]
        public virtual string FirstName { get; set; }
        [DisplayName("Last Name")]
        public virtual string LastName { get; set; }
        public virtual string Name { get { return string.IsNullOrWhiteSpace(string.Format("{0} {1}", FirstName, LastName)) ? Email : string.Format("{0} {1}", FirstName, LastName); } }
        public virtual byte[] PasswordHash { get; set; }
        public virtual byte[] PasswordSalt { get; set; }

        public virtual Guid Guid { get; set; }

        [Required]
        [Remote("IsUniqueEmail", "User", AdditionalFields = "Id")]
        public virtual string Email { get; set; }

        [Required]
        [DisplayName("Is Active")]
        public virtual bool IsActive { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }
        public virtual int LoginAttempts { get; set; }

        public virtual Guid? ResetPasswordGuid { get; set; }
        public virtual DateTime? ResetPasswordExpiry { get; set; }

        public virtual IList<UserRole> Roles { get; set; }
        protected internal virtual IList<UserProfileData> UserProfileData { get; set; }

        public virtual T Get<T>() where T : UserProfileData
        {
            return UserProfileData.OfType<T>().FirstOrDefault();
        }
        public virtual IEnumerable<T> GetAll<T>() where T : UserProfileData
        {
            return UserProfileData.OfType<T>();
        }

        public virtual bool IsAdmin
        {
            get { return Roles != null && Roles.Any(role => role.Name == UserRole.Administrator); }
        }

        public virtual IList<Site> Sites { get; set; }


        public override void OnDeleting(ISession session)
        {
            base.OnDeleting(session);
            foreach (var userRole in Roles)
                userRole.Users.Remove(this);
            Roles.Clear();
            foreach (var site in Sites)
                site.Users.Remove(this);
            Sites.Clear();
        }

        public virtual bool CanAccess<T>(string operation, string type = null) where T : ACLRule, new()
        {
            return new T().CanAccess(this, operation, type);
        }

        public static List<Type> OwnedObjectTypes
        {
            get { return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<IBelongToUser>(); }
        }
    }
}