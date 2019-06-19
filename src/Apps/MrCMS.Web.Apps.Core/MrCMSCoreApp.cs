using Microsoft.AspNetCore.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.Core.Filters;

namespace MrCMS.Web.Apps.Core
{
    public class MrCMSCoreApp : StandardMrCMSApp
    {
        public MrCMSCoreApp()
        {
            ContentPrefix = "/Apps/Core";
        }
        public override string Name => "Core";
        public override string Version => "1.0";

        public override void SetupMvcOptions(MvcOptions options)
        {
            options.Filters.Add<PasswordAuthFilter>();
        }
    }
}