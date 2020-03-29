using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public abstract class ItemBreadcrumb<TParent, TItem> : Breadcrumb<TParent>
        where TItem : SystemEntity where TParent : Breadcrumb
    {
        protected readonly IRepositoryBase<TItem> Repository;

        protected ItemBreadcrumb(IRepositoryBase<TItem> repository)
        {
            Repository = repository;
        }

        public override string Controller => typeof(TItem).Name;
        public override string Action => "Edit";
        public override bool IsNav => false;
        public override bool ShouldSkip => !Id.HasValue;

        protected virtual string GetName(TItem item)
        {
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

        public override async Task Populate()
        {
            if (Id.HasValue)
            {
                var item = await Repository.GetData<TItem>(Id.Value);
                Name = GetName(item);
            }
        }
    }
}