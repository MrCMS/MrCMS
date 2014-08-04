using System;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Website;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SetOnAddingProperties : IOnAdding
    {
        private readonly Site _site;

        public SetOnAddingProperties(Site site)
        {
            _site = site;
        }

        public void Execute(OnAddingArgs args)
        {
            DateTime now = CurrentRequestData.Now;
            SystemEntity systemEntity = args.Item;
            if (systemEntity.CreatedOn == DateTime.MinValue)
            {
                systemEntity.CreatedOn = now;
            }
            if (systemEntity.UpdatedOn == DateTime.MinValue)
            {
                systemEntity.UpdatedOn = now;
            }
            var siteEntity = systemEntity as SiteEntity;
            if (siteEntity == null) return;
            if (siteEntity.Site == null)
            {
                siteEntity.Site = _site;
            }
        }
    }
}