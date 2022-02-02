using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public class WebpageTypeIsValid : IWebpageImportValidationRule
    {
        private readonly IWebpageMetadataService _webpageMetadataService;

        public WebpageTypeIsValid(IWebpageMetadataService webpageMetadataService)
        {
            _webpageMetadataService = webpageMetadataService;
        }

        public Task<IReadOnlyList<string>> GetErrors(WebpageImportDTO item, IList<WebpageImportDTO> allItems)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(item.WebpageType) ||
                _webpageMetadataService.GetTypeByName(item.WebpageType) == null)
                list.Add("Webpage Type is not valid MrCMS type.");
            return Task.FromResult<IReadOnlyList<string>>(list);
        }
    }
}