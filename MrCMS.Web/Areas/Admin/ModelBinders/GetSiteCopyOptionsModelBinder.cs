using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            NameValueCollection form = controllerContext.HttpContext.Request.Form;
            IEnumerable<string> keys = form.AllKeys.Where(s => s.StartsWith("sco-"));

            foreach (string key in keys)
            {
                string value = form[key];
                int id;
                string typeName = key.Substring(4);
                Type type = TypeHelper.GetTypeByName(typeName);
                if (int.TryParse(value, out id) && type != null)
                {
                    siteCopyOptions.Add(new SiteCopyOption {SiteCopyActionType = type, SiteId = id});
                }
            }

            return siteCopyOptions;
        }
    }
}