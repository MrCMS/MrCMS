using System.Xml;
using System.Xml.Linq;
using MrCMS.Services.Sitemaps;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.SitemapGeneration
{
    public class HideResetPasswordPageFromSitemap : SitemapGenerationInfo<ResetPasswordPage>
    {
        public override bool ShouldAppend(ResetPasswordPage webpage)
        {
            return false;
        }

        public override void Append(ResetPasswordPage webpage, XElement urlset, XDocument xmlDocument)
        {
        }
    }
}