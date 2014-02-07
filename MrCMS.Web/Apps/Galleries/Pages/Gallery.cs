using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Galleries.Pages
{
    public class Gallery : TextPage
    {
        public override void AdminViewData(ViewDataDictionary viewData, ISession session)
        {
            viewData["galleries"] = session.QueryOver<MediaCategory>()
                                       .OrderBy(category => category.Name)
                                       .Desc.Cacheable()
                                       .List()
                                       .BuildSelectItemList(category => category.Name,
                                                            category => category.Id.ToString(),
                                                            emptyItemText: "Select a gallery...");
        }

        public virtual MediaCategory MediaGallery { get; set; }

        [DisplayName("Gallery Thumbnail Image")]
        public virtual string ThumbnailImage { get; set; }
    }
}