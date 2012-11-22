using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.AddOns.Pages
{
    [DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Contact Us", IconClass = "icon-file", DisplayOrder = 10, Type = typeof(ContactUsPage), WebGetAction = "Show", WebGetController = "ContactUs", WebPostController = "ContactUs", WebPostAction = "Submit")]
    [MrCMSMapClass]
    public class ContactUsPage : TextPage
    {

    }
}