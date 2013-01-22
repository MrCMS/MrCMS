using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Entities.People
{
    public class User : SystemEntity
    {
        [Required]
        [DisplayName("First Name")]
        public virtual string FirstName { get; set; }
        [DisplayName("Last Name")]
        public virtual string LastName { get; set; }
        public virtual string Name { get { return string.IsNullOrWhiteSpace(string.Format("{0} {1}", FirstName, LastName)) ? Email : string.Format("{0} {1}", FirstName, LastName); } }
        public virtual byte[] PasswordHash { get; set; }
        public virtual byte[] PasswordSalt { get; set; }

        [Required]
        public virtual string Email { get; set; }

        [Required]
        [DisplayName("Is Active")]
        public virtual bool IsActive { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }
        public virtual int LoginAttempts { get; set; }

        //public virtual MediaImage Avatar { get; set; }

        public virtual Guid? ResetPasswordGuid { get; set; }
        public virtual DateTime? ResetPasswordExpiry { get; set; }

        public virtual IList<UserRole> Roles { get; set; }

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
    }
}