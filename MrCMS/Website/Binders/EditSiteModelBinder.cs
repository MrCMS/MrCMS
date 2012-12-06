using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using NHibernate;

namespace MrCMS.Website.Binders
{
    public class EditSiteModelBinder : MrCMSDefaultModelBinder
    {
        public EditSiteModelBinder(ISession session)
            : base(() => session)
        {
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return Session.Get<Site>(Convert.ToInt32(controllerContext.HttpContext.Request["Id"]));
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var site = base.BindModel(controllerContext, bindingContext) as Site;

            IEnumerable<string> siteValues = controllerContext.HttpContext.Request.Params.AllKeys.Where(s => s.StartsWith("User-"));

            foreach (var value in siteValues)
            {
                string s = controllerContext.HttpContext.Request[value];
                var selected = s.Contains("true");
                var id = Convert.ToInt32(value.Split('-')[1]);

                var user = Session.Get<User>(id);

                if (selected && !user.Sites.Contains(site))
                {
                    user.Sites.Add(site);
                    site.Users.Add(user);
                }
                else if (!selected && user.Sites.Contains(site))
                {
                    user.Sites.Remove(site);
                    site.Users.Remove(user);
                }
            }

            return site;
        }
    }
}