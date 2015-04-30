using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Elmah;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using NHibernate;
using Ninject;

namespace MrCMS.Website
{
    public class CurrentRequestData
    {
        private const string UserSessionId = "current.usersessionGuid";
        private static bool? _databaseIsInstalled;

        public static ErrorSignal ErrorSignal
        {
            get { return (ErrorSignal) CurrentContext.Items["current.errorsignal"]; }
            set { CurrentContext.Items["current.errorsignal"] = value; }
        }

        public static User CurrentUser
        {
            get { return (User) CurrentContext.Items["current.user"]; }
            set { CurrentContext.Items["current.user"] = value; }
        }

        public static Site CurrentSite
        {
            get
            {
                return (Site) CurrentContext.Items["current.site"] ??
                       (CurrentSite = MrCMSApplication.Get<ICurrentSiteLocator>().GetCurrentSite());
            }
            set
            {
                CurrentContext.Items["current.site"] = value;
                SetSiteFilter(value);
            }
        }

        public static Webpage CurrentPage
        {
            get { return (Webpage) CurrentContext.Items["current.webpage"]; }
            set { CurrentContext.Items["current.webpage"] = value; }
        }

        public static SiteSettings SiteSettings
        {
            get { return (SiteSettings) CurrentContext.Items["current.sitesettings"]; }
            set { CurrentContext.Items["current.sitesettings"] = value; }
        }

        public static Webpage HomePage
        {
            get { return (Webpage) CurrentContext.Items["current.homepage"]; }
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
                //return TimeZoneInfo.Local;
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

        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                {
                    return MrCMSApplication.Get<IEnsureDatabaseIsInstalled>().IsInstalled();
                }
                return _databaseIsInstalled.Value;
            }
            set { _databaseIsInstalled = value; }
        }

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
                string o = CurrentContext.Request.Cookies[UserSessionId] != null
                    ? CurrentContext.Request.Cookies[UserSessionId].Value
                    : null;
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

        public static HashSet<EndRequestTask> OnEndRequest
        {
            get
            {
                return (HashSet<EndRequestTask>)(CurrentContext.Items["current.on-end-request"] ??
                                                   (CurrentContext.Items["current.on-end-request"] =
                                                       new HashSet<EndRequestTask>()));
            }
            set { CurrentContext.Items["current.on-end-request"] = value; }
        }

        private static void SetSiteFilter(Site value)
        {
            var session = MrCMSApplication.Get<ISession>();
            if (value != null)
            {
                session.EnableFilter("SiteFilter").SetParameter("site", value.Id);
            }
            else
            {
                IFilter enabledFilter = session.GetEnabledFilter("SiteFilter");
                if (enabledFilter != null)
                {
                    session.DisableFilter("SiteFilter");
                }
            }
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
    }
}