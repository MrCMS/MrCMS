using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class GetExistingTag : IGetExistingTag
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetExistingTag(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<Tag> GetTag(string name)
        {
            return
                (await _tagRepository.Query().ToListAsync())
                .Where(tag => tag.Name != null && tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Take(1)
                .SingleOrDefault();
        }
    }
}