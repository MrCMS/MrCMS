using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

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
            return
                _session.QueryOver<Tag>().Where(x => x.Name.IsInsensitiveLike(term, MatchMode.Start)).List().Select(
                    tag =>
                    new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = string.Format("{0}", tag.Name),
                            value = tag.Name
                        });
        }

        public IEnumerable<Tag> GetTags(Document document)
        {
            Iesi.Collections.Generic.ISet<Tag> parentCategories = new HashedSet<Tag>();

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