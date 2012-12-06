using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Entities.Settings;

namespace MrCMS.Entities.Multisite
{
    public class Site : BaseEntity
    {
        public virtual string Name { get; set; }

        [DisplayName("Base URL")]
        public virtual string BaseUrl { get; set; }

        public virtual IList<Webpage> Webpages { get; set; }

        public virtual IList<Setting> Settings { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}
