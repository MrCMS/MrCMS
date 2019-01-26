namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public interface IPageHeaderBreadcrumbBuilder
    {
        PageHeaderBreadcrumb Build(Breadcrumb breadcrumb);
    }
}