using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class TagAdminService : ITagAdminService
    {
        private readonly ISession _session;

        public TagAdminService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            AutoCompleteResult alias = null;
            return
                _session.QueryOver<Tag>()
                    .Where(x => x.Name.IsInsensitiveLike(term, MatchMode.Start))
                    .OrderBy(tag => tag.Name).Asc
                    .SelectList(builder =>
                    {
                        builder.Select(tag => tag.Id).WithAlias(() => alias.id);
                        builder.Select(tag => tag.Name).WithAlias(() => alias.label);
                        builder.Select(tag => tag.Name).WithAlias(() => alias.value);
                        return builder;
                    })
                    .TransformUsing(Transformers.AliasToBean<AutoCompleteResult>())
                    .List<AutoCompleteResult>();
        }
    }
}