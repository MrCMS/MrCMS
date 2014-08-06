using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class AddPropertiesModelBinder : MrCMSDefaultModelBinder
    {
        public AddPropertiesModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var typeName =
                GetValueFromContext(controllerContext, "type").Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries)
                    [0];
            var entityType = TypeHelper.MappedClasses.FirstOrDefault(type => type.FullName == typeName);

            if (entityType != null && entityType.HasDefaultConstructor())
            {
                var bindModel = Activator.CreateInstance(entityType) as Webpage;

                var parentId = GetValueFromContext(controllerContext, "parentId");
                int id;
                if (int.TryParse(parentId, out id) && bindModel != null)
                {
                    bindModel.Parent = Session.Get<Webpage>(id);
                }

                return bindModel;
            }
            return null;
        }
    }
}