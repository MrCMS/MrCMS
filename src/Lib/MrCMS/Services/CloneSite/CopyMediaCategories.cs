using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-80)]
    public class CopyMediaCategories : ICloneSiteParts
    {
        private readonly IGlobalRepository<MediaCategory> _repository;

        public CopyMediaCategories(IGlobalRepository<MediaCategory> repository)
        {
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            IEnumerable<MediaCategory> copies = GetMediaCategoryCopies(@from, to, siteCloneContext);

            await _repository.AddRange(copies.ToList());
        }

        private IEnumerable<MediaCategory> GetMediaCategoryCopies(Site @from, Site to, SiteCloneContext siteCloneContext, MediaCategory fromParent = null, MediaCategory toParent = null)
        {
            var parentId = fromParent?.Id;
            IList<MediaCategory> categories = _repository.Query().Where(layout => layout.Site.Id == @from.Id && layout.ParentId == parentId).ToList();
            foreach (MediaCategory category in categories)
            {
                MediaCategory copy = category.GetCopyForSite(to);
                siteCloneContext.AddEntry(category, copy);
                copy.Parent = toParent;
                yield return copy;
                foreach (MediaCategory child in GetMediaCategoryCopies(@from, to, siteCloneContext, fromParent: category, toParent: copy))
                {
                    yield return child;
                }
            }
        }
    }
}