using System;
using MrCMS.Apps;

namespace MrCMS.Web.Apps.WebAPIExample
{
    public class MrCmsWebApiSampleApp : StandardMrCMSApp
    {
        public MrCmsWebApiSampleApp()
        {
            ContentPrefix = "/Apps/WebApiSample";
            ViewPrefix = "/Apps/WebApiSample";
        }
        public override string Name => "WebApiSample";
        public override string Version => "1.0";
    }
}
