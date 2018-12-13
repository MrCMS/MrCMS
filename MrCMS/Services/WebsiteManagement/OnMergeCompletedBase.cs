using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.WebsiteManagement
{
    public abstract class OnMergeCompletedBase
    {
        public abstract void MergeCompleted(Webpage @from, Webpage to);
    }

    public abstract class OnMergeCompletedBase<TWebpage> : OnMergeCompletedBase where TWebpage : Webpage
    {
        public abstract void MergeCompleted(TWebpage webpage, Webpage to);
        public override void MergeCompleted(Webpage @from, Webpage to)
        {
            MergeCompleted(@from as TWebpage, to);
        }
    }
}