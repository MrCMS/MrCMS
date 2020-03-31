using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class TagAdminService : ITagAdminService
    {
        private readonly IRepository<Tag> _documentTagRepository;

        public TagAdminService(IRepository<Tag> documentTagRepository)
        {
            _documentTagRepository = documentTagRepository;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            return
                _documentTagRepository.Readonly()
                    .Where(x => EF.Functions.Like(x.Name, $"{term}%"))
                    .OrderBy(tag => tag.Name)
                    .Distinct()
                    .Select(dt =>
                        new AutoCompleteResult
                        {
                            id = dt.Id,
                            label = dt.Name,
                            value = dt.Name
                        })
                    .ToList();
        }
    }
}