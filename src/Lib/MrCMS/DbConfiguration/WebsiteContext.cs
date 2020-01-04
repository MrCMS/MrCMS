using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration
{
    public class WebsiteContext : DbContext, ISystemDatabase
    {
        private readonly IGetModelCreators _getModelCreators;

        public WebsiteContext(IGetModelCreators getModelCreators, DbContextOptions<WebsiteContext> options)
            : base(options)
        {
            _getModelCreators = getModelCreators;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var modelCreator in _getModelCreators.GetCreators())
                modelCreator.Create(modelBuilder);

            // use non-pluralised tablenames
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());

                // Replace column names            
                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.Name);
            }
        }
    }

    //public class WebpageConfiguration : IEntityTypeConfiguration<Webpage>
    //{
    //    public void Configure(EntityTypeBuilder<Webpage> builder)
    //    {
    //        builder.HasDiscriminator<string>(CreateCoreModel.WebpageDiscriminator);
    //    }
    //}
}