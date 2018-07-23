using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web
{
    //[DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Redirect", IconUrl = "icon-forward", DisplayOrder = 6, Type = typeof(Redirect))]
    public class Redirect : Webpage
    {
        [DisplayName("Redirect Url")]
        public virtual string RedirectUrl { get; set; }
        [DisplayName("Is Permanent")]
        public virtual bool Permanent { get; set; }

    }
}
