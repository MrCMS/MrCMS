using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.CloneSite
{
    public abstract class CloneWebpagePart
    {
        public abstract Task ClonePartBase(Webpage @from, Webpage to, SiteCloneContext siteCloneContext);
    }

    public abstract class CloneWebpagePart<T> : CloneWebpagePart where T : Webpage
    {
        public override sealed async Task ClonePartBase(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (!(@from is T pageFrom) || !(to is T pageTo))
                return;
            await ClonePart(pageFrom, pageTo, siteCloneContext);
        }

        public abstract Task ClonePart(T @from, T to, SiteCloneContext siteCloneContext);
    }
}