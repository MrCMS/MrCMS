using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IReadOnlyList<Document>> Get(int? parent)
        {
            var list = new List<Document>();
            if (parent > 0)
            {
                var document = await _session.GetAsync<Document>(parent);
                while (document != null)
                {
                    list.Add(document);
                    document = document.Parent;
                }
            }
            list.Reverse();
            return list;
        }
    }
}