using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.WebsiteManagement
{
    public abstract class OnMergingWebpagesBase
    {
        public abstract void MergeCompleted(Webpage @from, Webpage to);
    }

    public abstract class OnMergingWebpagesBase<TWebpage> : OnMergingWebpagesBase where TWebpage : Webpage
    {
        public abstract void MergeCompleted(TWebpage webpage, Webpage to);
        public override void MergeCompleted(Webpage @from, Webpage to)
        {
            MergeCompleted(@from as TWebpage, to);
        }
    }
}