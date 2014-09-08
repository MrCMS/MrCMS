using System;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class DocumentTagsAdminService : IDocumentTagsAdminService
    {
        private readonly ISession _session;

        public DocumentTagsAdminService(ISession session)
        {
            _session = session;
        }

        public void SetTags(string taglist, Document document)
        {
            if (document == null) throw new ArgumentNullException("document");

            if (taglist == null)
                taglist = string.Empty;

            var tagNames =
                taglist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x));

            var existingTags = document.Tags.ToList();

            tagNames.ForEach(name =>
            {
                var tag = GetTag(name);
                if (tag == null)
                {
                    tag = new Tag {Name = name};
                    _session.Transact(session => session.Save(tag));
                }
                if (!document.Tags.Contains(tag))
                {
                    document.Tags.Add(tag);
                    tag.Documents.Add(document);
                }
                existingTags.Remove(tag);
            });

            existingTags.ForEach(tag =>
            {
                document.Tags.Remove(tag);
                tag.Documents.Remove(document);
            });
        }

        private Tag GetTag(string name)
        {
            return _session.QueryOver<Tag>().Where(tag => tag.Name.IsLike(name, MatchMode.Exact)).Take(1).SingleOrDefault();
        }
    }
}