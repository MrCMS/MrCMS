using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateFormModel : ICreateModel
    {
        private readonly IReflectionHelper _reflectionHelper;

        public CreateFormModel(IReflectionHelper reflectionHelper)
        {
            _reflectionHelper = reflectionHelper;
        }
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Form>();

            builder.Entity<FormProperty>(formProperty =>
            {
                var discriminatorBuilder = formProperty.HasDiscriminator<string>("PropertyType");
                foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf<FormProperty>())
                    discriminatorBuilder.HasValue(type, type.FullName);
            });
        }
    }
}