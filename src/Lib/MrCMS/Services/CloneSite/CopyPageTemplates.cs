using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-85)]
    public class CopyPageTemplates : ICloneSiteParts
    {
        private readonly IGlobalRepository<PageTemplate> _repository;

        public CopyPageTemplates(IGlobalRepository<PageTemplate> repository)
        {
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var existingTemplates =
                _repository.Query().Where(template => template.Site.Id == @from.Id).ToList();

            await _repository.Transact(async (repo, ct) =>
             {
                 foreach (var template in existingTemplates)
                 {
                     var copy = template.GetCopyForSite(to);
                     if (template.Layout != null)
                         copy.Layout = siteCloneContext.FindNew<Layout>(template.Layout.Id);
                     await repo.Add(copy, ct);
                     siteCloneContext.AddEntry(template, copy);
                 }
             });
        }
    }
}