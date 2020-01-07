using System;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public class GetExistingTag : IGetExistingTag
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetExistingTag(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag GetTag(string name)
        {
            return
                _tagRepository.Query().ToList()
                    .Where(tag => tag.Name != null && tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .Take(1)
                    .SingleOrDefault();
        }
    }
}