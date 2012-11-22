using System.Text;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Controllers
{
    public class SEOController : Controller
    {
        private readonly INavigationService _navigationService;
        private readonly SEOSettings _seoSettings;

        public SEOController(INavigationService navigationService, SEOSettings seoSettings)
        {
            _navigationService = navigationService;
            _seoSettings = seoSettings;
        }

        public ActionResult Sitemap()
        {
            var content = _navigationService.GetSiteMap(Url);
            return Content(content, "application/xml", Encoding.UTF8);
        }

        public ActionResult Robots()
        {
            return Content(_seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }

        public ActionResult RenderAnalytics()
        {
            if (string.IsNullOrWhiteSpace(_seoSettings.GoogleAnalytics))
                return new EmptyResult();

            var format = string.Format(@"<script type=""text/javascript"">
            var _gaq = _gaq || [];
            _gaq.push(['_setAccount', '{0}']);
            _gaq.push(['_trackPageview']);

            (function () {{
                var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
                ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
            }})();
        </script>", _seoSettings.GoogleAnalytics);

            return Content(format);
        }
    }
}