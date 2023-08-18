using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;

namespace MrCMS.Web.Admin.Models
{
    public class RedirectViewModel : IUpdatePropertiesViewModel<Redirect>, IAddPropertiesViewModel<Redirect>
    {
        public string RedirectUrl { get; set; }
        public bool Permanent { get; set; }
        public bool RevealInNavigation { get; set; }
    }
}