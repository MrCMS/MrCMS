using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
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
            var categories = GetCategories(document);

            return
                _session.QueryOver<Tag>().Where(x => x.Site == document.Site && x.Name.IsInsensitiveLike(term, MatchMode.Start)).List().Select(
                    tag =>
                    new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = string.Format("{0}{1}", tag.Name, (categories.Contains(tag) ? " (Category)" : string.Empty)),
                            value = tag.Name
                        });
        }

        public IEnumerable<Tag> GetCategories(Document document)
        {
            IList<Tag> parentCategories = new List<Tag>();

            if (document != null)
            {
                if (document.Parent != null && document.Parent.Unproxy() is IDocumentContainer)
                    parentCategories = document.Parent.Tags;
            }

            return parentCategories;
        }
    }
}