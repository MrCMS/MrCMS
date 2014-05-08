using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public interface IOnWebpageUnpublished : IEvent<OnWebpageUnpublishedEventArgs>
    {
    }

    public class OnWebpageUnpublishedEventArgs
    {
        public OnWebpageUnpublishedEventArgs(Webpage webpage)
        {
            Webpage = webpage;
        }

        public Webpage Webpage { get; set; }
    }
}