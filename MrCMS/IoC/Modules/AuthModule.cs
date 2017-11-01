using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using MrCMS.Services;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    public class AuthModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IAuthenticationManager>().ToMethod(context => context.Kernel.Get<HttpContextBase>().GetOwinContext().Authentication);
            Kernel.Bind<IEnumerable<IHashAlgorithm>>()
                .ToMethod(context => context.Kernel.GetAll<IHashAlgorithm>())
                .InRequestScope();
            Kernel.Bind<IEnumerable<IExternalUserSource>>()
                .ToMethod(context => context.Kernel.GetAll<IExternalUserSource>())
                .InRequestScope();
            //Kernel.Bind<IUserStore<User, int>>().To<UserStore>().InRequestScope();
            Kernel.Rebind<IUserManager>().ToMethod(context =>
            {
                var userManager = new UserManager(context.Kernel.Get<IUserStore>());
                userManager.UserValidator = new UserValidator<User, int>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                };
                return userManager;
            }).InRequestScope();

        }
    }
}