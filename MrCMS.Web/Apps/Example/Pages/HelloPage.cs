using System.ComponentModel;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Example.Pages
{
    public class HelloPage : Webpage
    {
        [DisplayName("Hello to")]
        public virtual string HelloTo { get; set; }
    }

}