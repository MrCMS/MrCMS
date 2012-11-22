using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Entities.People
{
    public class User : BaseEntity
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
    }
}