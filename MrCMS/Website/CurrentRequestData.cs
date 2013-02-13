using System.Web;
using Elmah;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class CurrentRequestData
    {
        public static ErrorSignal ErrorSignal
        {
            get { return (ErrorSignal)CurrentContext.Items["current.errorsignal"]; }
            set { CurrentContext.Items["current.errorsignal"] = value; }
        }

        public static User CurrentUser
        {
            get { return (User)CurrentContext.Items["current.user"]; }
            set { CurrentContext.Items["current.user"] = value; }
        }

        public static Site CurrentSite
        {
            get { return (Site)CurrentContext.Items["current.site"]; }
            set { CurrentContext.Items["current.site"] = value; }
        }

        public static Webpage CurrentPage
        {
            get { return (Webpage)CurrentContext.Items["current.webpage"]; }
            set { CurrentContext.Items["current.webpage"] = value; }
        }

        public static SiteSettings SiteSettings
        {
            get { return (SiteSettings)CurrentContext.Items["current.sitesettings"]; }
            set { CurrentContext.Items["current.sitesettings"] = value; }
        }

        public static HttpContextBase CurrentContext
        {
            get { return OverridenContext ?? new HttpContextWrapper(HttpContext.Current); }
        }

        public static HttpContextBase OverridenContext { get; set; }

        public static bool CurrentUserIsAdmin
        {
            get { return CurrentUser != null && CurrentUser.IsAdmin; }
        }
    }
}