using System;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.AddOns.Pages.Blogs
{
    [DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Blog", RequiresParent = true, AutoBlacklist = true, IconClass = "icon-edit", DisplayOrder = 1, Type = typeof(BlogPage), WebGetAction = "Show", WebGetController = "Blog", SortBy = "CreatedOn", SortByDesc = true, DefaultLayoutName = "Two Column")]
    [MrCMSMapClass]
    public class BlogPage : TextPage, IContainerItem
    {
        //private DateTime _articleDate = DateTime.Now;

        [AllowHtml]
        public virtual string Abstract { get; set; }

        public virtual string Image { get; set; }

        public virtual User Author { get; set; }

        public virtual string AuthorName
        {
            get { return Author != null ? Author.Name : string.Empty; }
        }

        //public virtual DateTime ArticleDate { get { return _articleDate; } set { _articleDate = value; } }
        public virtual string ContainerUrl
        {
            get
            {
                var documentContainer = Parent as IDocumentContainer<BlogPage>;
                return documentContainer == null ? null : documentContainer.LiveUrlSegment;
            }
        }
    }
}