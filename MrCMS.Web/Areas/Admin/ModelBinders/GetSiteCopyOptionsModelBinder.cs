using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website.Binders;
using NHibernate.Mapping;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class GetSiteCopyOptionsModelBinder : MrCMSDefaultModelBinder
    {
        public GetSiteCopyOptionsModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var siteCopyOptions = new List<SiteCopyOption>();

            var form = controllerContext.HttpContext.Request.Form;
            var keys = form.AllKeys.Where(s => s.StartsWith("sco-"));
            
            foreach (var key in keys)
            {
                var value = form[key];
                int id;
                var typeName = key.Substring(4);
                var type = TypeHelper.GetTypeByName(typeName);
                if (int.TryParse(value, out id) && type != null)
                {
                    siteCopyOptions.Add(new SiteCopyOption {SiteCopyActionType = type, SiteId = id});
                }
            }

            return siteCopyOptions;
        }
    }
}