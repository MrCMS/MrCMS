using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Controllers
{
    public class WebpageController : MrCMSUIController
    {
        [CanonicalLinks]
        public ViewResult Show(Webpage page)
        {
            return View(page);
        }
    }
}