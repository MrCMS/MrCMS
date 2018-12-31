using System.ComponentModel;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Apps.Core.Pages
{
    //[WebpageOutputCacheable(CacheExpiryType.Sliding, 3600)]
    public class TextPage : Webpage
    {
        [DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }
    }
}