using Microsoft.EntityFrameworkCore;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateSiteModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Site>();
            builder.Entity<RedirectedDomain>();
        }
    }
}