using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class TagService : ITagService
    {
        private readonly ISession _session;

        public TagService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<AutoCompleteResult> Search(Document document, string term)
        {
            var tags = GetTags(document);

            return
                _session.QueryOver<Tag>().Where(x => x.Site == document.Site && x.Name.IsInsensitiveLike(term, MatchMode.Start)).List().Select(
                    tag =>
                    new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = string.Format("{0}{1}", tag.Name, (tags.Contains(tag) ? " (Category)" : string.Empty)),
                            value = tag.Name
                        });
        }

        public IEnumerable<Tag> GetTags(Document document)
        {
            IList<Tag> parentCategories = new List<Tag>();

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