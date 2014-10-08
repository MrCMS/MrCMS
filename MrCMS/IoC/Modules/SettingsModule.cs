using System.Linq;
using MrCMS.Helpers;
using MrCMS.Settings;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.Syntax;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    public class SettingsModule : NinjectModule
    {
        private readonly bool _forTest;

        public SettingsModule(bool forTest = false)
        {
            _forTest = forTest;
        }

        public override void Load()
        {
            Kernel.Bind(syntax =>
            {
                IJoinExcludeIncludeBindSyntax joinExcludeIncludeBindSyntax = syntax.From(
                    TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                    .Where(
                        t =>
                            typeof(SiteSettingsBase).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any());

                if (_forTest)
                {
                    joinExcludeIncludeBindSyntax.BindToSelf();
                }
                else
                {
                    joinExcludeIncludeBindSyntax
                        .BindWith<NinjectSiteSettingsBinder>()
                        .Configure(onSyntax => onSyntax.InRequestScope());
                }
            });
            Kernel.Bind(syntax =>
            {
                IJoinExcludeIncludeBindSyntax joinExcludeIncludeBindSyntax = syntax.From(
                    TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                    .Where(
                        t =>
                            typeof(SystemSettingsBase).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any());

                if (_forTest)
                {
                    joinExcludeIncludeBindSyntax.BindToSelf();
                }
                else
                {
                    joinExcludeIncludeBindSyntax
                        .BindWith<NinjectSystemSettingsBinder>()
                        .Configure(onSyntax => onSyntax.InRequestScope());
                }
            });
        }
    }
}