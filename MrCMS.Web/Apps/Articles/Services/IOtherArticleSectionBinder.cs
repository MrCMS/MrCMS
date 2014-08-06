using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IOtherArticleSectionBinder
    {
        void Bind(ControllerContext context, Article article);
    }
}