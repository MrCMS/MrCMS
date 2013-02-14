using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Application.Pages
{
    //[DocumentTypeDefinition(ChildrenListType.BlackList, Name = "Search Page", IconClass = "icon-file", DisplayOrder = 1, Type = typeof(SearchPage), WebGetAction = "View", WebGetController = "SearchPage", WebPostAction = "Post", WebPostController = "SearchPage", DefaultLayoutName = "Three Column")]
    [MrCMSMapClass]
    public class SearchPage : TextPage
    {
        
    }
}