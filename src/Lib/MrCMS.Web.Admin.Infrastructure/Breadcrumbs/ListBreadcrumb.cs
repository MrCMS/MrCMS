using MrCMS.Entities;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public abstract class ListBreadcrumb<T> : Breadcrumb where T : SystemEntity//, IHaveId
    {
        protected ListBreadcrumb(int order)
        {
            Order = order;
        }

        public override string Controller => typeof(T).Name;
        public override string Action => "Index";
        public override bool IsNav => true;
        public override decimal Order { get; }
    }
}