using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Widget;
using System;
using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface IWidgetUIService
    {
        Task<IHtmlContent> GetContent(IViewComponentHelper helper, int id,
            Func<IViewComponentHelper, Task<IHtmlContent>> func);
        Task<object> GetModel(Widget widget);
        Task<Widget> GetWidget(int id);
    }
}