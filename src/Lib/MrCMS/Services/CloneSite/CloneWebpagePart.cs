using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.CloneSite
{
    public abstract class CloneWebpagePart
    {
        public abstract void ClonePartBase(Webpage @from, Webpage to, SiteCloneContext siteCloneContext);
    }

    public abstract class CloneWebpagePart<T> : CloneWebpagePart where T : Webpage
    {
        public override sealed void ClonePartBase(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            var pageFrom = @from as T;
            var pageTo = to as T;
            if (pageFrom == null || pageTo == null)
                return;
            ClonePart(pageFrom, pageTo, siteCloneContext);
        }

        public abstract void ClonePart(T @from, T to, SiteCloneContext siteCloneContext);
    }
}