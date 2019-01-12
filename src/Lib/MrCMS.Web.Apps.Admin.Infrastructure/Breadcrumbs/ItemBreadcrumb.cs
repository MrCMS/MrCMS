using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public abstract class ItemBreadcrumb<TParent, TItem> : Breadcrumb<TParent>
        where TItem : SystemEntity where TParent : Breadcrumb
    {
        protected readonly ISession Session;

        protected ItemBreadcrumb(ISession session)
        {
            Session = session;
        }

        public override string Controller => typeof(TItem).Name;
        public override string Action => "Edit";
        public override bool IsNav => false;
        public override bool ShouldSkip => !Id.HasValue;

        protected virtual string GetName(TItem item)
        {
            item = item.Unproxy();
            if (item == null)
                return "-";
            var type = item.GetType();
            var property = type.GetProperty("Name");

            string DefaultName()
            {
                return type.Name.BreakUpString() + " - Id:" + item.Id;
            }

            if (property == null)
                return DefaultName();
            return property.GetValue(item)?.ToString() ?? DefaultName();
        }

        public override void Populate()
        {
            if (Id.HasValue)
            {
                var item = Session.Get<TItem>(Id.Value);
                Name = GetName(item);
            }
        }
    }
}