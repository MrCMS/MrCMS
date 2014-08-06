using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageBaseViewDataService : IWebpageBaseViewDataService
    {
        private readonly ISession _session;
        private readonly IValidWebpageChildrenService _validWebpageChildrenService;

        public WebpageBaseViewDataService(IValidWebpageChildrenService validWebpageChildrenService, ISession session)
        {
            _validWebpageChildrenService = validWebpageChildrenService;
            _session = session;
        }

        public void SetAddPageViewData(ViewDataDictionary viewData, Webpage parent)
        {
            IOrderedEnumerable<DocumentMetadata> validWebpageDocumentTypes = _validWebpageChildrenService
                .GetValidWebpageDocumentTypes(parent,
                    metadata =>
                        CurrentRequestData.CurrentUser.CanAccess<TypeACLRule>(TypeACLRule.Add, metadata.Type.FullName))
                .OrderBy(metadata => metadata.DisplayOrder);

            List<string> typeNames = validWebpageDocumentTypes.Select(metadata => metadata.Type.FullName).ToList();

            IList<PageTemplate> templates =
                _session.QueryOver<PageTemplate>().Where(template => template.PageType.IsIn(typeNames))
                    .OrderBy(template => template.Name).Asc.Cacheable().List();

            var documentTypeToAdds = new List<DocumentTypeToAdd>();

            foreach (DocumentMetadata type in validWebpageDocumentTypes)
            {
                if (templates.Any(template => template.PageType == type.Type.FullName))
                {
                    documentTypeToAdds.Add(new DocumentTypeToAdd
                    {
                        Type = type,
                        DisplayName = string.Format("Default {0}", type.Name)
                    });
                    DocumentMetadata typeCopy = type;
                    documentTypeToAdds.AddRange(
                        templates.Where(template => template.PageType == typeCopy.Type.FullName)
                            .Select(pageTemplate => new DocumentTypeToAdd
                            {
                                Type = type,
                                DisplayName = pageTemplate.Name,
                                TemplateId = pageTemplate.Id
                            }));
                }
                else
                {
                    documentTypeToAdds.Add(new DocumentTypeToAdd {Type = type, DisplayName = type.Name});
                }
            }

            viewData["DocumentTypes"] = documentTypeToAdds;
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