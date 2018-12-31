using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public class OnWebpagePublishedEventArgs
    {
        public OnWebpagePublishedEventArgs(Webpage document)
        {
            Webpage = document;
        }

        public Webpage Webpage { get; set; }
    }
}