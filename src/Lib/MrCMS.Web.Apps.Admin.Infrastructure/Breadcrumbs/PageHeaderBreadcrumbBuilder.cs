namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class PageHeaderBreadcrumbBuilder : IPageHeaderBreadcrumbBuilder
    {
        public PageHeaderBreadcrumb Build(Breadcrumb breadcrumb)
        {
            if (breadcrumb == null)
                return null;
            return new PageHeaderBreadcrumb
            {
                BreadcrumbType = breadcrumb.GetType(),
                Title = breadcrumb.Title,
                Name = breadcrumb.Name,
                Link = breadcrumb.Url
            };
        }
    }
}