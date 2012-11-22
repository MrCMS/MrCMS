using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.AddOns.Pages
{
    [DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Gallery", IconClass = "icon-th-large", DisplayOrder = 5, Type = typeof(Gallery), WebGetController = "Gallery", WebGetAction = "View")]
    [MrCMSMapClass]
    public class Gallery : TextPage
    {
        private IList<GalleryItem> _items = new List<GalleryItem>();
        public virtual IList<GalleryItem> Items
        {
            get { return _items; }
            protected internal set { _items = value; }
        }
    }
}
