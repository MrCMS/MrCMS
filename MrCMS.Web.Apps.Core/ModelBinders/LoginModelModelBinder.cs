using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Models.Auth;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.ModelBinders
{
    public class LoginModelModelBinder : IExtendedModelBinder
    {
        private readonly IModelBinder _modelBinder;

        public LoginModelModelBinder(IModelBinder modelBinder)
        {
            _modelBinder = modelBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _modelBinder.BindModelAsync(bindingContext);
            if (!bindingContext.Result.IsModelSet) return;

            var foo = bindingContext.Result.Model as LoginModel;
            if (foo == null)
                throw new InvalidOperationException(
                    $"Expected {bindingContext.ModelName} to have been bound by ComplexTypeModelBinder");

            if (foo.Email != null)
                foo.Email = foo.Email.Trim();
        }
    }

    //public class LoginModelBinderProvider : IModelBinderProvider
    //{
    //    private readonly IModelBinderProvider _workerProvider;

    //    public LoginModelBinderProvider(IModelBinderProvider workerProvider)
    //    {
    //        _workerProvider = workerProvider;
    //    }

    //    public IModelBinder GetBinder(ModelBinderProviderContext context)
    //    {
    //        if (context == null) throw new ArgumentNullException(nameof(context));

    //        if (context.Metadata.ModelType == typeof(LoginModel))
    //            return new LoginModelModelBinder(_workerProvider.GetBinder(context));

    //        return null;
    //    }
    //}
}