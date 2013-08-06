using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.People;

namespace MrCMS.Entities.Multisite
{
    public class Site : SystemEntity
    {
        public Site()
        {
        }
        [Required]
        public virtual string Name { get; set; }

        [DisplayName("Base URL")]
        [Required]
        public virtual string BaseUrl { get; set; }

        public virtual bool IsValidForSite(SiteEntity entity)
        {
            return entity.Site != null && entity.Site.Id == Id;
        }
    }
}
