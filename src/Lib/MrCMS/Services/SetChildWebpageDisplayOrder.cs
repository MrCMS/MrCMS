using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class SetChildWebpageDisplayOrder : IOnAdding<Webpage>
    {
        public async Task Execute(OnAddingArgs<Webpage> args)
        {
            Webpage webpage = args.Item;
            // if the document isn't set or it's top level (i.e. no parent) we don't want to deal with it here
            if (webpage?.Parent == null)
                return;

            // if it's not 0 it means it's been set, so we'll not update it
            if (webpage.DisplayOrder != 0)
                return;

            webpage.DisplayOrder = await GetMaxParentDisplayOrder(webpage.Parent, args.Session);
        }

        private async Task<int> GetMaxParentDisplayOrder(Webpage parent, ISession session)
        {
            if (await session.QueryOver<Webpage>()
                    .Where(doc => doc.Parent.Id == parent.Id)
                    .AnyAsync())
                return await session.QueryOver<Webpage>()
                    .Where(doc => doc.Parent.Id == parent.Id)
                    .Select(Projections.Max<Webpage>(d => d.DisplayOrder))
                    .SingleOrDefaultAsync<int>() + 1;
            return 0;
        }
    }
}