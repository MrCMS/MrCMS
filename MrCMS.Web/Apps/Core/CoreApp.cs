using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Iesi.Collections.Generic;
using Microsoft.AspNet.Identity;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Core
{
    public class CoreApp : MrCMSApp
    {
        private readonly CoreAppInstallation _coreAppInstallation = new CoreAppInstallation();

        public override string AppName
        {
            get { return "Core"; }
        }

        public override string Version
        {
            get { return MrCMSApplication.AssemblyVersion; }
        }

        protected override int InstallOrder
        {
            get { return 1; }
        }

        protected override void RegisterServices(IKernel kernel)
        {

        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            CoreAppInstallation.Install(session, model, site);
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("External Login", "external-login", new { controller = "ExternalLogin", action = "Login" });
            context.MapRoute("External Login Callback", "external-login/callback", new { controller = "ExternalLogin", action = "Callback" });

            context.MapRoute("User Registration", "Registration/RegistrationDetails", new { controller = "Registration", action = "RegistrationDetails" });
            context.MapRoute("User Registration - check email", "Registration/CheckEmailIsNotRegistered", new { controller = "Registration", action = "CheckEmailIsNotRegistered" });

            context.MapRoute("UserAccountController - account details", "UserAccount/UserAccountDetails", new { controller = "UserAccount", action = "UserAccountDetails" });
            context.MapRoute("UserAccountController - check email isn't already registered", "UserAccount/IsUniqueEmail", new { controller = "UserAccount", action = "IsUniqueEmail" });

            context.MapRoute("UserAccountController - change password", "UserAccount/ChangePassword", new { controller = "UserAccount", action = "ChangePassword" });
        }
    }
}