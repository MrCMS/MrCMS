using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class GetValidPageTemplatesToAdd : IGetValidPageTemplatesToAdd
    {
        private readonly ISession _session;

        public GetValidPageTemplatesToAdd(ISession session)
        {
            _session = session;
        }

        public List<PageTemplate> Get(IEnumerable<DocumentMetadata> validWebpageDocumentTypes)
        {
            List<string> typeNames = validWebpageDocumentTypes.Select(metadata => metadata.Type.FullName).ToList();

            List<PageTemplate> templates =
                _session.QueryOver<PageTemplate>().Where(template => template.PageType.IsIn(typeNames))
                    .OrderBy(template => template.Name).Asc.Cacheable().List().ToList();

            templates = templates.FindAll(template =>
            {
                if (!template.SingleUse)
                    return true;
                return !_session.QueryOver<Webpage>().Where(webpage => webpage.PageTemplate.Id == template.Id).Any();
            });
            return templates;
        }
    }
}