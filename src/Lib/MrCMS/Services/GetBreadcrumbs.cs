using System.Collections.Generic;
using MrCMS.Entities.Documents;
using NHibernate;

namespace MrCMS.Services
{
    public class GetBreadcrumbs : IGetBreadcrumbs
    {
        private readonly ISession _session;

        public GetBreadcrumbs(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Document> Get(int? parent)
        {
            if (parent > 0)
            {
                var document = _session.Get<Document>(parent);
                while (document != null)
                {
                    yield return document;
                    document = document.Parent;
                }
            }
        }

    }
}