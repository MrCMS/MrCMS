using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class WebpageExtensions
    {


        public static bool CanAddChildren(this Webpage webpage)
        {
            return webpage.GetMetadata().ValidChildrenTypes.Any();
        }
    }
}