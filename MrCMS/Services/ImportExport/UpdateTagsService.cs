using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Services.ImportExport
{
    public class UpdateTagsService : IUpdateTagsService
    {
        private readonly ISession _session;

        public UpdateTagsService(ISession session)
        {
            _session = session;
        }

        public void SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            List<string> tagsToAdd =
                documentDto.Tags.Where(
                    s => !webpage.Tags.Select(tag => tag.Name).Contains(s, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            List<Tag> tagsToRemove =
                webpage.Tags.Where(
                    tag => !documentDto.Tags.Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (string item in tagsToAdd)
            {
                Tag tag = GetExistingTag(item);
                bool isNew = tag == null;
                if (isNew)
                {
                    tag = new Tag {Name = item};
                    _session.Transact(session => session.Save(tag));
                }
                if (!webpage.Tags.Contains(tag))
                    webpage.Tags.Add(tag);

                if (!tag.Documents.Contains(webpage))
                    tag.Documents.Add(webpage);
                _session.Transact(session => session.Update(tag));
            }

            foreach (Tag tag in tagsToRemove)
            {
                webpage.Tags.Remove(tag);
                tag.Documents.Remove(webpage);
                Tag closureTag = tag;
                _session.Transact(session => session.Update(closureTag));
            }
        }

        private Tag GetExistingTag(string item)
        {
            return
                _session.QueryOver<Tag>()
                    .Where(tag => tag.Name.IsInsensitiveLike(item, MatchMode.Exact))
                    .Take(1)
                    .SingleOrDefault();
        }
    }
}