using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Controllers
{
    public class WebpageController : MrCMSUIController
    {
        public ViewResult Show(Webpage page)
        {
            return View(page);
        }
    }
}