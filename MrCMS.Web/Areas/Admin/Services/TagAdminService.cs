using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
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

        public IEnumerable<AutoCompleteResult> Search( string term)
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

        public IEnumerable<Tag> GetTags(Document document)
        {
            ISet<Tag> parentCategories = new HashSet<Tag>();

            if (document != null)
            {
                if (document.Parent != null)
                    parentCategories = document.Parent.Tags;
            }

            return parentCategories;
        }

        public Tag GetByName(string name)
        {
            return _session.QueryOver<Tag>().Where(x => x.Site == CurrentRequestData.CurrentSite
                && x.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }
        public void Add(Tag tag)
        {
            _session.Transact(session => session.Save(tag));
        }
    }
}