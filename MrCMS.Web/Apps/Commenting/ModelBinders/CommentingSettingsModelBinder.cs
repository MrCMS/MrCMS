using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.Settings;
using MrCMS.Website.Binders;
using NHibernate;
using System.Linq;
using Ninject;

namespace MrCMS.Web.Apps.Commenting.ModelBinders
{
    public class CommentingSettingsModelBinder : MrCMSDefaultModelBinder
    {
        public const string PageType = "page-type-";
        private readonly IConfigurationProvider _configurationProvider;

        public CommentingSettingsModelBinder(IKernel kernel)
            : base(kernel)
        {
            _configurationProvider = kernel.Get<IConfigurationProvider>();
        }
        public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            var commentingSettings = bindModel as CommentingSettings;
            if (commentingSettings == null)
                return bindModel;
            var types = new List<Type>();
            foreach (var source in controllerContext.HttpContext.Request.Params.AllKeys.Where(s => s.StartsWith(PageType)))
            {
                var value = controllerContext.HttpContext.Request[source];
                if (value.Contains("true"))
                    types.Add(TypeHelper.GetTypeByName(source.Replace(PageType, string.Empty)));
            }
            commentingSettings.SetAllowedPageTypes(types.ToArray());
            return commentingSettings;
        }

        protected override object CreateModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, Type modelType)
        {
            return _configurationProvider.GetSiteSettings<CommentingSettings>();
        }
    }
}