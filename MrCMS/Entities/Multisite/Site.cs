using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.People;

namespace MrCMS.Entities.Multisite
{
    public class Site : SystemEntity
    {
        public Site()
        {
            Users = new List<User>();
        }
        public virtual string Name { get; set; }

        [DisplayName("Base URL")]
        public virtual string BaseUrl { get; set; }

        public virtual IList<User> Users { get; set; }


        public virtual bool IsValidForSite(SiteEntity entity)
        {
            return entity.Site != null && entity.Site.Id == Id;
        }
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
