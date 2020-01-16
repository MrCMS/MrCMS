using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class ExistAnyWebpageService : IExistAnyWebpageService
    {
        private readonly IRepository<Webpage> _repository;

        private Dictionary<string, int> _counts;

        public ExistAnyWebpageService(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public bool ExistAny(Type type)
        {
            _counts = (_counts ?? GetCounts());
            return _counts.ContainsKey(type?.FullName ?? string.Empty) && _counts[type.FullName] > 0;
        }

        private Dictionary<string, int> GetCounts()
        {
            var webpageCounts =
                _repository.Readonly()
                    .GroupBy(x => x.DocumentClrType).Select(g => new
                    {
                        Type = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                        .Where(type => !webpageCounts.Select(count => count.Type).Contains(type.FullName)))
            {
                webpageCounts.Add(new { Type = type.FullName, Count = 0 });
            }
            return webpageCounts.ToDictionary(count => count.Type, count => count.Count);
        }
    }
}