using MrCMS.Apps;

namespace MrCMS.Web.Apps.Articles
{
    public class MrCMSArticlesApp : StandardMrCMSApp
    {
        public MrCMSArticlesApp()
        {
            ContentPrefix = "/Apps/Articles";
        }
        public override string Name => "Articles";
        public override string Version => "1.0";
    }
}
