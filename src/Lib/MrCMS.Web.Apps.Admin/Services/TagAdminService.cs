using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class TagAdminService : ITagAdminService
    {
        private readonly IJoinTableRepository<DocumentTag> _documentTagRepository;

        public TagAdminService(IJoinTableRepository<DocumentTag> documentTagRepository)
        {
            _documentTagRepository = documentTagRepository;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            return
                _documentTagRepository.Readonly()
                    .Where(x => EF.Functions.Like(x.Tag.Name, $"{term}%"))
                    .OrderBy(tag => tag.Tag.Name)
                    .Select(dt =>
                        new AutoCompleteResult
                        {
                            id = dt.TagId,
                            label = dt.Tag.Name,
                            value = dt.Tag.Name
                        })
                    .ToList();
        }
    }
}