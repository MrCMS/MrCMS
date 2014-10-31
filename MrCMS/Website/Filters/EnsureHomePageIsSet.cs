using System.Web.Mvc;
using MrCMS.Services;

namespace MrCMS.Website.Filters
{
    public class EnsureHomePageIsSet : ActionFilterAttribute
    {
        private IGetHomePage _getHomePage;

        public IGetHomePage GetHomePage
        {
            get { return _getHomePage ?? MrCMSApplication.Get<IGetHomePage>(); }
            set { _getHomePage = value; }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentRequestData.HomePage == null)
                CurrentRequestData.HomePage = GetHomePage.Get();
        }
    }
}