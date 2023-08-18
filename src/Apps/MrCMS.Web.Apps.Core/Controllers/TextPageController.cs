using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;
using StackExchange.Profiling;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TextPageController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IWebpageUIService _uiService;

        public TextPageController(IWebpageUIService uiService)
        {
            _uiService = uiService;
        }

        [CanonicalLinks]
        public async Task<ActionResult> Show(int id)
        {
            using (MiniProfiler.Current.Step("TextPageController.Show"))
            {
                var page = await _uiService.GetPage<TextPage>(id);
                return View(page);
            }
        }
    }
}