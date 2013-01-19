using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;

namespace MrCMS.Entities.Multisite
{
    public class Site : SystemEntity
    {
        public virtual string Name { get; set; }

        [DisplayName("Base URL")]
        public virtual string BaseUrl { get; set; }

        public virtual IList<User> Users { get; set; }
    }

    [DoNotMap]
    public class CurrentSite : Site
    {
        public CurrentSite(Site site)
        {
            Name = site.Name;
            BaseUrl = site.BaseUrl;
            Id = site.Id;
            Site = site;
        }

        public Site Site { get; set; }
    }

}
