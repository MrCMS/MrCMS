using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class EditWebpageModelBinder : IModelBinder
    {

        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //{
        //}

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var session = serviceProvider.GetRequiredService<ISession>();
            var firstValue = bindingContext.ValueProvider.GetValue("id").FirstValue;
            if (!int.TryParse(firstValue, out int id))
            {
                return;
            }

            var webpage = session.Get<Webpage>(id);
            if (webpage == null)
            {
                return;
            }

            var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();
            var typeArguments = webpage.GetType();
            bindingContext.ModelMetadata = metadataProvider.GetMetadataForType(typeArguments);
            bindingContext.Model = webpage;
            //var instance = SystemEntityBinderProvider.GetModelBinder(bindingContext.ModelMetadata, serviceProvider);
            //await instance.BindModelAsync(bindingContext);

            string frontEndRoles = bindingContext.ValueProvider.GetValue("FrontEndRoles").FirstValue;
            var documentRolesAdminService = serviceProvider.GetRequiredService<IDocumentRolesAdminService>();
            documentRolesAdminService.SetFrontEndRoles(frontEndRoles, webpage);
        }
    }
}