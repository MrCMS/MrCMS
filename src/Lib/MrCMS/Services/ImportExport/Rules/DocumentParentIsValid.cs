using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentParentIsValid : IDocumentImportValidationRule
    {
        private readonly IGetWebpageByUrl<Webpage> _getWebpageByUrl;

        public DocumentParentIsValid(IGetWebpageByUrl<Webpage> getWebpageByUrl)
        {
            _getWebpageByUrl = getWebpageByUrl;
        }

        public async Task<IReadOnlyList<string>> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(item.ParentUrl) && await _getWebpageByUrl.GetByUrl(item.ParentUrl) == null && allItems.All(x => x.UrlSegment != item.ParentUrl))
                list.Add("The parent url specified is not present within the system.");
            return list;
        }
    }
}