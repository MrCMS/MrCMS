using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace MrCMS.Apps
{
    public class MrCMSAppRegistrationContext
    {
        private readonly HashSet<string> _namespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public MrCMSAppRegistrationContext(string appName, RouteCollection routes, object state = null)
        {
            if (appName == null) throw new ArgumentNullException("appName");
            if (routes == null)
                throw new ArgumentNullException("routes");
            AppName = appName;
            Routes = routes;
            State = state;
        }

        public string AppName { get; private set; }
        public ICollection<string> Namespaces
        {
            get { return _namespaces; }
        }
        public RouteCollection Routes { get; private set; }
        public object State { get; private set; }

        public Route MapRoute(string name, string url, object defaults = null)
        {
            return MapRoute(name, url, defaults, (object)null);
        }

        public Route MapRoute(string name, string url, string[] namespaces)
        {
            return MapRoute(name, url, null, namespaces);
        }

        public Route MapRoute(string name, string url, object defaults, string[] namespaces)
        {
            return MapRoute(name, url, defaults, null, namespaces);
        }

        public Route MapRoute(string name, string url, object defaults, object constraints, string[] namespaces = null)
        {
            if (namespaces == null && Namespaces != null)
                namespaces = Namespaces.ToArray();
            Route route = Routes.MapRoute(name, url, defaults, constraints, namespaces);
            route.DataTokens["app"] = AppName;
            bool flag = namespaces == null || namespaces.Length == 0;
            route.DataTokens["UseNamespaceFallback"] = (flag ? 1 : 0);
            return route;
        }
        public Route MapAreaRoute(string name, string areaName, string url, object defaults = null)
        {
            return MapAreaRoute(name, areaName, url, defaults, (object)null);
        }

        public Route MapAreaRoute(string name, string areaName, string url, string[] namespaces)
        {
            return MapAreaRoute(name, areaName, url, null, namespaces);
        }

        public Route MapAreaRoute(string name, string areaName, string url, object defaults, string[] namespaces)
        {
            return MapAreaRoute(name, areaName, url, defaults, null, namespaces);
        }

        public Route MapAreaRoute(string name, string areaName, string url, object defaults, object constraints, string[] namespaces = null)
        {
            if (namespaces == null && Namespaces != null)
                namespaces = Namespaces.ToArray();
            Route route = Routes.MapRoute(name, url, defaults, constraints, namespaces);
            route.DataTokens["app"] = AppName;
            route.DataTokens["area"] = areaName;
            bool flag = namespaces == null || namespaces.Length == 0;
            route.DataTokens["UseNamespaceFallback"] = (flag ? 1 : 0);
            return route;
        }
    }
}