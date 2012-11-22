using System.ComponentModel;
using MrCMS.AddOns.Pages;
using MrCMS.AddOns.Pages.Blogs;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class LatestBlogPostsWidget : Widget
    {
        [DisplayName("Blog Container")]
        public virtual BlogContainer BlogContainer { get; set; }
        public virtual int Count { get; set; }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, NHibernate.ISession session)
        {
            viewData["BlogContainers"] =
                session.QueryOver<BlogContainer>().OrderBy(container => container.Name).Asc.List().BuildSelectItemList(
                    container => container.Name, container => container.Id.ToString(),
                    emptyItem: SelectListItemHelper.EmptyItem("Select a blog container"));
        }
    }
}