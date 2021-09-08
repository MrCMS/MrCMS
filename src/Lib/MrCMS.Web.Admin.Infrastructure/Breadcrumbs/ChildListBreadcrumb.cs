namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public abstract class ChildListBreadcrumb<TParent, T> : Breadcrumb<TParent> where T : class/*, IHaveId */where TParent : Breadcrumb
    {
        protected ChildListBreadcrumb(int order)
        {
            Order = order;
        }

        public override string Controller => typeof(T).Name;
        public override string Action => "Index";
        public override bool IsNav => true;
        public override decimal Order { get; }
    }
}