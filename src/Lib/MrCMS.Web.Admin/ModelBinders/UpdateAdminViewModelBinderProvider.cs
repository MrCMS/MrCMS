using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class UpdateAdminViewModelBinderProvider : IModelBinderProvider
    {
        private static readonly HashSet<Type> ModelTypes =
            TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(UpdateAdminViewModel<>));

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (ModelTypes.Contains(context.Metadata.ModelType))
                return new UpdateAdminViewModelBinder(context.CreateBinder);

            return null;
        }
    }
}