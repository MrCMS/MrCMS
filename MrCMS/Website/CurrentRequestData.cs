using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Elmah;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Services;
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
            get
            {
                return (Site) CurrentContext.Items["current.site"] ??
                       (CurrentSite = MrCMSApplication.Get<ISiteService>().GetCurrentSite());
            }
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

        private static bool? _databaseIsInstalled;

        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                {
                    var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;

                    var connectionStrings = Path.Combine(applicationPhysicalPath, "ConnectionStrings.config");

                    if (!File.Exists(connectionStrings))
                    {
                        File.WriteAllText(connectionStrings, "<connectionStrings></connectionStrings>");
                    }

                    var connectionString = ConfigurationManager.ConnectionStrings["mrcms"];
                    _databaseIsInstalled = connectionString != null &&
                                           !String.IsNullOrEmpty(connectionString.ConnectionString);
                }
                return _databaseIsInstalled.Value;
            }
            set { _databaseIsInstalled = value; }
        }

        public static Guid UserGuid
        {
            get
            {
                if (CurrentUser != null) return CurrentUser.Guid;
                var o = CurrentContext.Session["current.usersessionGuid"];
                return (Guid)(o != null ? (Guid)o : (CurrentContext.Session["current.usersessionGuid"] = Guid.NewGuid()));
            }
            set { CurrentContext.Session["current.usersessionGuid"] = value; }
        }
    }
}