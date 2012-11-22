using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.AddOns.Pages.Portfolio
{
    [MrCMSMapClass]
    [DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Portfolio Item", RequiresParent = true, AutoBlacklist = true, IconClass = "icon-edit", DisplayOrder = 9, Type = typeof(PortfolioItem), WebGetAction = "Show", WebGetController = "Portfolio", DefaultLayoutName = "Portfolio Item")]
    public class PortfolioItem : TextPage, IContainerItem
    {
        public virtual IList<PortfolioImage> Images { get; set; }

        public virtual string WebsiteUrl { get; set; }

        public virtual PortfolioImage PrimaryImage { get { return Images.FirstOrDefault(image => image.IsPrimaryImage); } }

        public virtual PortfolioContainer Container { get { return Parent.Unproxy() as PortfolioContainer; } }
        public virtual string ContainerUrl
        {
            get
            {
                var documentContainer = Parent as IDocumentContainer<PortfolioItem>;
                return documentContainer == null ? null : documentContainer.LiveUrlSegment;
            }
        }

        public override void OnSaving(NHibernate.ISession session)
        {
            Images.ForEach(session.SaveOrUpdate);
        }
    }
}