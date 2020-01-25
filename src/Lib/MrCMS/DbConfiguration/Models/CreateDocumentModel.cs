using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateDocumentModel : ICreateModel
    {
        private readonly IReflectionHelper _reflectionHelper;

        public CreateDocumentModel(IReflectionHelper reflectionHelper)
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

                document.HasMany(x => x.DocumentTags);
            });
            //builder.Entity<Webpage>(webpage => { webpage.Ignore(x => x.ActivePages); });
            builder.Entity<PageWidgetSort>();
            builder.Entity<HiddenWidget>(hiddenWidget =>
            {
                hiddenWidget.ToTable("HiddenWidgets");
                hiddenWidget.HasKey(x => new {x.WidgetId, x.WebpageId});
            });
            builder.Entity<ShownWidget>(shownWidget =>
            {
                shownWidget.ToTable("ShownWidgets");
                shownWidget.HasKey(x => new {x.WidgetId, x.WebpageId});
            });

            builder.Entity<DocumentTag>(documentTag => documentTag.HasKey(x => new { x.DocumentId, x.TagId }));
            builder.Entity<FrontEndAllowedRole>(frontEndAllowedRole =>
            {
                frontEndAllowedRole.HasKey(x => new { x.RoleId, x.WebpageId });
                frontEndAllowedRole.Property(x => x.RoleId).HasColumnName("UserRoleId");
                frontEndAllowedRole.HasOne(x => x.Webpage).WithMany(x => x.FrontEndAllowedRoles);
            });

            builder.Entity<Tag>(typeBuilder => typeBuilder.HasMany(x => x.DocumentTags));

            builder.Entity<Widget>(widget =>
            {
                var discriminatorBuilder = widget.HasDiscriminator<string>("WidgetType");
                foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf<Widget>())
                    discriminatorBuilder.HasValue(type, type.FullName);
            });

            builder.Entity<ContentBlock>(contentBlock =>
            {
                var discriminatorBuilder = contentBlock.HasDiscriminator<string>("discriminator");
                foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf<ContentBlock>())
                    discriminatorBuilder.HasValue(type, type.FullName);
            });

            builder.Entity<DocumentVersion>();
        }
    }
}