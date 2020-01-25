using Microsoft.EntityFrameworkCore;
using MrCMS.Indexing.Management;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateSearchModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<LuceneFieldBoost>();
        }
    }
}