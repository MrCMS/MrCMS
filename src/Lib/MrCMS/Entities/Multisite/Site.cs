using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MrCMS.Website;
using System.Linq;

namespace MrCMS.Entities.Multisite
{
    public class Site : SystemEntity
    {
        public Site()
        {
            RedirectedDomains = new List<RedirectedDomain>();
        }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string BaseUrl { get; set; }

        public virtual string StagingUrl { get; set; }
        
        public virtual IList<RedirectedDomain> RedirectedDomains { get; set; }

        public virtual string DisplayName => $"{Name} ({BaseUrl})";

        public virtual bool IsValidForSite(SiteEntity entity)
        {
            if (entity.GetType().GetCustomAttributes(typeof(AdminUISiteAgnosticAttribute), true).Any())
                return true;
            return entity.Site != null && entity.Site.Id == Id;
        }
    }
}
