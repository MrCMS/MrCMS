using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
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

        public IEnumerable<AutoCompleteResult> Search(string term, int documentId)
        {
            var document = _session.Get<Document>(documentId);
            IList<Tag> parentCategories = new List<Tag>();

            if (document != null && document.Parent != null && typeof(IDocumentContainer<>).IsInstanceOfType(document.Parent))
                parentCategories = document.Parent.Tags;

            return
                _session.QueryOver<Tag>().Where(x => x.Name.IsLike(term, MatchMode.Start)).List().Select(
                    tag =>
                    new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = string.Format("{0}{1}", tag.Name, (parentCategories.Contains(tag) ? " (Category)" : string.Empty)),
                            value = tag.Name
                        });
        }
    }
}