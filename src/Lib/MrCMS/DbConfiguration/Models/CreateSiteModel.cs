using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
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
    public class CreateWebpageModel : ICreateModel
    {
        private readonly IReflectionHelper _reflectionHelper;

        public CreateWebpageModel(IReflectionHelper reflectionHelper)
        {
            _reflectionHelper = reflectionHelper;
        }
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Document>(document =>
            {
                var discriminatorBuilder = document.HasDiscriminator<string>("DocumentType");
                foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf<Document>())
                    discriminatorBuilder.HasValue(type, type.FullName);
            });
            builder.Entity<Webpage>().HasBaseType<Document>();
        }
    }
}