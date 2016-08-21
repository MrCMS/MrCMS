using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class TagAdminService : ITagAdminService
    {
        private readonly IRepository<Tag> _tagRepository;

        public TagAdminService(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<AutoCompleteResult>();
            return
                _tagRepository.Query()
                    .OrderBy(tag => tag.Name)
                    .Select(tag =>
                        new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = tag.Name,
                            value = tag.Name
                        })
                    .ToList().Where(x => x.value.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
    }
}