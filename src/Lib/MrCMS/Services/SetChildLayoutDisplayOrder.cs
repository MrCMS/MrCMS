using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Events;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services;

public class SetChildLayoutDisplayOrder : IOnAdding<Layout>
{
    public async Task Execute(OnAddingArgs<Layout> args)
    {
        Layout webpage = args.Item;
        // if the document isn't set or it's top level (i.e. no parent) we don't want to deal with it here
        if (webpage?.Parent == null)
            return;

        // if it's not 0 it means it's been set, so we'll not update it
        if (webpage.DisplayOrder != 0)
            return;

        webpage.DisplayOrder = await GetMaxParentDisplayOrder(webpage.Parent, args.Session);
    }

    private async Task<int> GetMaxParentDisplayOrder(Layout parent, ISession session)
    {
        return await session.QueryOver<Layout>()
            .Where(doc => doc.Parent.Id == parent.Id)
            .Select(Projections.Max<Layout>(d => d.DisplayOrder))
            .SingleOrDefaultAsync<int>();
    }
}