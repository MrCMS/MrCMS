using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services.CloneSite
{
    public class SetPageTemplate : CloneWebpagePart<Webpage>
    {
        private readonly IGlobalRepository<Webpage> _repository;

        public SetPageTemplate(IGlobalRepository<Webpage> repository)
        {
            _repository = repository;
        }


        public override async Task ClonePart(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (@from.PageTemplate == null) 
                return;
            to.PageTemplate = siteCloneContext.FindNew<PageTemplate>(@from.PageTemplate.Id);
            await _repository.Update(to);
        }
    }
}