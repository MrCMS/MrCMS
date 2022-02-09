using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web
{
    public class Redirect : Webpage
    {
        [DisplayName("Redirect Url")]
        public virtual string RedirectUrl { get; set; }
        [DisplayName("Is Permanent")]
        public virtual bool Permanent { get; set; }
    }
}
