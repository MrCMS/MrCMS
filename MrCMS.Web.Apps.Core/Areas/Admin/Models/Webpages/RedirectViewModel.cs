using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Webpages
{
    public class RedirectViewModel : IUpdatePropertiesViewModel<Redirect>, IAddPropertiesViewModel<Redirect>
    {
        public string RedirectUrl { get; set; }
        public bool Permanent { get; set; }
        public bool RevealInNavigation { get; set; }
    }
}