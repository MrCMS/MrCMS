using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class EditUserModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            //var instance = SystemEntityBinderProvider.GetModelBinder(bindingContext.ModelMetadata, serviceProvider);
            //var session = serviceProvider.GetRequiredService<ISession>();
            //bindingContext.Model = session
            //    .Get<User>(Convert.ToInt32(bindingContext.ValueProvider.GetValue("Id")));
            //await instance.BindModelAsync(bindingContext);
            //var user = bindingContext.Result.Model as User;
            //if (user == null)
            //    return;

            //IEnumerable<string> roleValues = bindingContext.HttpContext.Request.Form.Keys.Where(s => s.StartsWith("Role-"));

            //foreach (var value in roleValues)
            //{
            //    string s = bindingContext.HttpContext.Request.Form[value];
            //    var roleSelected = s.Contains("true");
            //    var id = Convert.ToInt32(value.Split('-')[1]);

            //    var role = session.Get<UserRole>(id);
            //    if (serviceProvider.GetRequiredService<IRoleService>().IsOnlyAdmin(user) && role.IsAdmin)
            //        continue;

            //    if (roleSelected && !user.Roles.Contains(role))
            //    {
            //        user.Roles.Add(role);
            //        role.Users.Add(user);
            //    }
            //    else if (!roleSelected && user.Roles.Contains(role))
            //    {
            //        user.Roles.Remove(role);
            //        role.Users.Remove(user);
            //    }
            //}
        }
    }
}