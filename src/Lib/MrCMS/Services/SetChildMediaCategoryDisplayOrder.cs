using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Events;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services;

public class SetChildMediaCategoryDisplayOrder : IOnAdding<MediaCategory>
{
    public async Task Execute(OnAddingArgs<MediaCategory> args)
    {
        MediaCategory webpage = args.Item;
        // if the document isn't set or it's top level (i.e. no parent) we don't want to deal with it here
        if (webpage?.Parent == null)
            return;

        // if it's not 0 it means it's been set, so we'll not update it
        if (webpage.DisplayOrder != 0)
            return;

        webpage.DisplayOrder = await GetMaxParentDisplayOrder(webpage.Parent, args.Session);
    }

    private async Task<int> GetMaxParentDisplayOrder(MediaCategory parent, ISession session)
    {
        return await session.QueryOver<MediaCategory>()
            .Where(doc => doc.Parent.Id == parent.Id)
            .Select(Projections.Max<MediaCategory>(d => d.DisplayOrder))
            .SingleOrDefaultAsync<int>();
    }
}