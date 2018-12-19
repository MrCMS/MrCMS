using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;

namespace MrCMS.ViewComponents
{
    public class ContentBlockViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(ContentBlock block)
        {
            var contentBlock = block.Unproxy();
            return View(contentBlock.GetType().Name, contentBlock);
        }
    }
}