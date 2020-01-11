using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;

namespace MrCMS.ViewComponents
{
    public class ContentBlockViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(ContentBlock block)
        {
            return View(block.GetType().Name, block);
        }
    }
}