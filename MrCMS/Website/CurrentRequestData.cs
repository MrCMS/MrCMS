using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Elmah;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using Ninject;

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
                return (Site)CurrentContext.Items["current.site"] ??
                       (CurrentSite = MrCMSApplication.Get<ICurrentSiteLocator>().GetCurrentSite());
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

        public static Webpage HomePage
        {
            get { return (Webpage)CurrentContext.Items["current.homepage"]; }
            set { CurrentContext.Items["current.homepage"] = value; }
        }

        public static CultureInfo CultureInfo
        {
            get
            {
                return SiteSettings != null
                           ? CurrentContext.Items["current.cultureinfo"] as CultureInfo ??
                             (CurrentContext.Items["current.cultureinfo"] = SiteSettings.CultureInfo) as CultureInfo
                           : CultureInfo.CurrentCulture;
            }
        }

        public static TimeZoneInfo TimeZoneInfo
        {
            get
            {
                return SiteSettings != null
                           ? (SiteSettings.TimeZoneInfo ?? TimeZoneInfo.Local)
                           : TimeZoneInfo.Local;
            }
        }

        public static DateTime Now
        {
            get { return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo); }
        }

        public static HttpContextBase CurrentContext
        {
            get { return MrCMSApplication.Get<HttpContextBase>(); }
        }

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

        private const string UserSessionId = "current.usersessionGuid";
        private static Guid? UserGuidOverride
        {
            get { return CurrentContext.Items[UserSessionId] as Guid?; }
            set { CurrentContext.Items[UserSessionId] = value; }
        }
        public static Guid UserGuid
        {
            get
            {
                if (UserGuidOverride.HasValue)
                    return UserGuidOverride.Value;
                if (CurrentUser != null) return CurrentUser.Guid;
                var o = CurrentContext.Request.Cookies[UserSessionId] != null ? CurrentContext.Request.Cookies[UserSessionId].Value : null;
                Guid result;
                if (o == null || !Guid.TryParse(o, out result))
                {
                    result = Guid.NewGuid();
                    AddCookieToResponse(UserSessionId, result.ToString(), Now.AddMonths(3));
                }
                return result;
            }
            set { UserGuidOverride = value; }
        }
        private static void AddCookieToResponse(string key, string value, DateTime expiry)
        {
            var userGuidCookie = new HttpCookie(key)
            {
                Value = value,
                Expires = expiry
            };
            CurrentContext.Response.Cookies.Add(userGuidCookie);
        }

        public static HashSet<Action<IKernel>> OnEndRequest
        {
            get
            {
                return (HashSet<Action<IKernel>>)(CurrentContext.Items["current.on-end-request"] ??
                               (CurrentContext.Items["current.on-end-request"] = new HashSet<Action<IKernel>>()));
            }
            set { CurrentContext.Items["current.on-end-request"] = value; }
        }

        public static HashSet<QueuedTask> QueuedTasks
        {
            get
            {
                return (HashSet<QueuedTask>)(CurrentContext.Items["current.queued-tasks"] ??
                               (CurrentContext.Items["current.queued-tasks"] = new HashSet<QueuedTask>()));
            }
            set { CurrentContext.Items["current.queued-tasks"] = value; }
        }
    }
}