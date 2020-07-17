namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public interface IPageHeaderBreadcrumbBuilder
    {
        PageHeaderBreadcrumb Build(Breadcrumb breadcrumb);
    }
}