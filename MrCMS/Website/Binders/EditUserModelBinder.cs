using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using System.Linq;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class EditUserModelBinder : MrCMSDefaultModelBinder
    {
        public EditUserModelBinder(IKernel kernel) : base(kernel)
        {
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return Session.Get<User>(Convert.ToInt32(controllerContext.HttpContext.Request["Id"]));
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var user = base.BindModel(controllerContext, bindingContext) as User;

            IEnumerable<string> roleValues = controllerContext.HttpContext.Request.Params.AllKeys.Where(s => s.StartsWith("Role-"));

            foreach (var value in roleValues)
            {
                string s = controllerContext.HttpContext.Request[value];
                var roleSelected = s.Contains("true");
                var id = Convert.ToInt32(value.Split('-')[1]);

                var role = Session.Get<UserRole>(id);
                if (MrCMSApplication.Get<IRoleService>().IsOnlyAdmin(user) && role.IsAdmin)
                    continue;

                if (roleSelected && !user.Roles.Contains(role))
                {
                    user.Roles.Add(role);
                    role.Users.Add(user);
                }
                else if (!roleSelected && user.Roles.Contains(role))
                {
                    user.Roles.Remove(role);
                    role.Users.Remove(user);
                }
            }
            return user;
        }
    }
}