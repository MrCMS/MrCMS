using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageBaseViewDataService : IWebpageBaseViewDataService
    {
        private readonly IValidWebpageChildrenService _validWebpageChildrenService;

        public WebpageBaseViewDataService(IValidWebpageChildrenService validWebpageChildrenService)
        {
            _validWebpageChildrenService = validWebpageChildrenService;
        }

        public void SetAddPageViewData(ViewDataDictionary viewData, Webpage parent)
        {
            List<DocumentMetadata> documentTypeDefinitions =
                _validWebpageChildrenService.GetValidWebpageDocumentTypes(parent, metadata =>
                    CurrentRequestData.CurrentUser.CanAccess<TypeACLRule>(TypeACLRule.Add, metadata.Type.FullName))
                    .ToList();

            viewData["DocumentTypes"] = documentTypeDefinitions;
        }

        public void SetEditPageViewData(ViewDataDictionary viewData, Webpage page)
        {
            DocumentMetadata documentMetadata = page.GetMetadata();
            if (documentMetadata != null)
            {
                viewData["EditView"] = documentMetadata.EditPartialView;
            }
        }
    }
}