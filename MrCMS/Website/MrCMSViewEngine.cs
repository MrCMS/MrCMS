using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using StackExchange.Profiling;

namespace MrCMS.Website
{
    public abstract class MrCMSViewEngine : IViewEngine
    {
        private const string _cacheKeyPrefix_Master = "Master";
        private const string _cacheKeyPrefix_Partial = "Partial";
        private const string _cacheKeyPrefix_View = "View";
        // format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:"
        private const string _cacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:{6}:";
        private static readonly List<string> _emptyLocations = new List<string>();
        public IViewLocationCache ViewLocationCache { get; set; }
        internal Func<string, string> GetExtensionThunk = VirtualPathUtility.GetExtension;

        private IViewPageActivator _viewPageActivator;

        protected IViewPageActivator ViewPageActivator
        {
            get
            {
                if (_viewPageActivator != null)
                {
                    return _viewPageActivator;
                }
                _viewPageActivator = new DefaultViewPageActivator();
                return _viewPageActivator;
            }
            set { _viewPageActivator = value; }
        }

        private VirtualPathProvider _vpp;

        protected VirtualPathProvider VirtualPathProvider
        {
            get { return _vpp ?? (_vpp = HostingEnvironment.VirtualPathProvider); }
            set { _vpp = value; }
        }

        public virtual ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
            bool useCache)
        {
            using (MiniProfiler.Current.Step("Find Partial View " + partialViewName))
            {
                if (controllerContext == null)
                {
                    throw new ArgumentNullException("controllerContext");
                }
                if (string.IsNullOrEmpty(partialViewName))
                {
                    throw new ArgumentException("Argument null or empty", "partialViewName");
                }

                List<string> searched;
                string controllerName = controllerContext.RouteData.GetRequiredString("controller");

                string partialPath = GetPath(controllerContext, ViewEngineFileLocations.PartialViewLocationFormats,
                    ViewEngineFileLocations.AreaPartialViewLocationFormats,
                    ViewEngineFileLocations.AppPartialViewLocationFormats, "PartialViewLocationFormats", partialViewName,
                    controllerName,
                    _cacheKeyPrefix_Partial, useCache, out searched);

                if (string.IsNullOrEmpty(partialPath))
                {
                    return new ViewEngineResult(searched);
                }

                return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
            }
        }

        public virtual ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
            bool useCache)
        {
            using (MiniProfiler.Current.Step("Find View " + viewName + ", Master " + masterName))
            {
                if (controllerContext == null)
                {
                    throw new ArgumentNullException("controllerContext");
                }
                if (string.IsNullOrEmpty(viewName))
                {
                    throw new ArgumentException("Argument null or empty", "viewName");
                }

                List<string> viewLocationsSearched;
                List<string> masterLocationsSearched;

                string controllerName = controllerContext.RouteData.GetRequiredString("controller");
                string viewPath = GetPath(controllerContext, ViewEngineFileLocations.ViewLocationFormats,
                    ViewEngineFileLocations.AreaViewLocationFormats, ViewEngineFileLocations.AppViewLocationFormats,
                    "ViewLocationFormats", viewName, controllerName, _cacheKeyPrefix_View, useCache,
                    out viewLocationsSearched);
                string masterPath = GetPath(controllerContext, ViewEngineFileLocations.MasterLocationFormats,
                    ViewEngineFileLocations.AreaMasterLocationFormats, ViewEngineFileLocations.AppMasterLocationFormats,
                    "MasterLocationFormats", masterName, controllerName, _cacheKeyPrefix_Master,
                    useCache, out masterLocationsSearched);

                if (string.IsNullOrEmpty(viewPath) ||
                    (string.IsNullOrEmpty(masterPath) && !string.IsNullOrEmpty(masterName)))
                {
                    return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
                }

                return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
            }
        }

        public virtual void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var disposable = view as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        protected abstract IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath);

        protected abstract IView CreatePartialView(ControllerContext controllerContext, string partialPath);

        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations,
            string[] appLocations, string locationsPropertyName, string name, string controllerName,
            string cacheKeyPrefix, bool useCache, out List<string> searchedLocations)
        {
            using (MiniProfiler.Current.Step("Get Path"))
            {
                searchedLocations = _emptyLocations;

                if (string.IsNullOrEmpty(name))
                {
                    return string.Empty;
                }

                string areaName = GetAreaName(controllerContext.RouteData);
                string appName = GetAppName(controllerContext.RouteData);
                string themeName = GetThemeName(controllerContext.RouteData.DataTokens);
                bool usingAreas = !string.IsNullOrEmpty(areaName);
                bool usingApps = !string.IsNullOrEmpty(appName);
                IEnumerable<string> locationsByPriority =
                    ((usingApps) ? appLocations : new string[] { }).Union((usingAreas)
                        ? areaLocations
                        : new string[] { })
                        .Union(locations);
                List<ViewLocation> viewLocations = GetViewLocations(locationsByPriority.ToList(), themeName);

                if (viewLocations.Count == 0)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        "Property {0} cannot be null or empty",
                        locationsPropertyName));

                bool nameRepresentsPath = IsSpecificPath(name);
                string cacheKey = CreateCacheKey(cacheKeyPrefix, name,
                    (nameRepresentsPath) ? string.Empty : controllerName,
                    areaName, appName, themeName);

                return useCache
                    ? GetFromCache(controllerContext, cacheKey)
                    : ((nameRepresentsPath)
                        ? GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations)
                        : GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName,
                            appName, themeName, cacheKey, ref searchedLocations));
            }
        }

        private string GetFromCache(ControllerContext controllerContext, string cacheKey)
        {
            using (MiniProfiler.Current.Step("Get from cache: " + cacheKey))
            {
                return ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey);
            }
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations,
            string name, string controllerName, string areaName, string appName, string themeName, string cacheKey,
            ref List<string> searchedLocations)
        {
            string result = string.Empty;
            searchedLocations = new List<string>();
            List<string> appNames = MrCMSApp.AppNames.OrderBy(s => s == appName ? 0 : 1).ToList();

            foreach (ViewLocation location in locations)
            {
                foreach (string app in appNames)
                {
                    string virtualPath = location.Format(name, controllerName, areaName, app, themeName);

                    if (FileExists(virtualPath))
                    {
                        searchedLocations = _emptyLocations;
                        result = virtualPath;
                        ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
                        return result;
                    }

                    if (!searchedLocations.Contains(virtualPath))
                        searchedLocations.Add(virtualPath);
                }
            }

            return result;
        }

        protected abstract bool FileExists(string virtualPath);

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey,
            ref List<string> searchedLocations)
        {
            string result = name;

            if (!(FilePathIsSupported(name) && FileExists(name)))
            {
                result = string.Empty;
                searchedLocations = new List<string> { name };
            }

            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        protected bool FilePathIsSupported(string virtualPath)
        {
            if (ViewEngineFileLocations.FileExtensions == null)
            {
                // legacy behavior for custom ViewEngine that might not set the FileExtensions property
                return true;
            }

            // get rid of the '.' because the FileExtensions property expects extensions withouth a dot.
            string extension = GetExtensionThunk(virtualPath).TrimStart('.');
            return ViewEngineFileLocations.FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string areaName, string appName,
            string themeName)
        {
            return string.Format(CultureInfo.InvariantCulture, _cacheKeyFormat,
                GetType().AssemblyQualifiedName, prefix, name, controllerName, areaName, appName,
                themeName);
        }

        private static List<ViewLocation> GetViewLocations(List<string> locationsByPriority, string themeName)
        {
            var allLocations = new List<ViewLocation>();

            foreach (string location in locationsByPriority)
            {
                if (!string.IsNullOrWhiteSpace(themeName))
                    allLocations.Add(
                        new ViewLocation(string.Format("{0}Themes/{1}/{2}", location.Substring(0, 2), themeName,
                            location.Substring(2))));
                allLocations.Add(new ViewLocation(location));
            }

            return allLocations;
        }

        private static bool IsSpecificPath(string name)
        {
            char c = name[0];
            return (c == '~' || c == '/');
        }

        private static string GetAreaName(RouteBase route)
        {
            var routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
                return routeWithArea.Area;
            var route1 = route as Route;
            return route1 != null && route1.DataTokens != null
                ? route1.DataTokens["area"] as string
                : null;
        }

        private static string GetAreaName(RouteData routeData)
        {
            object obj;
            return routeData.DataTokens.TryGetValue("area", out obj)
                ? obj as string
                : GetAreaName(routeData.Route);
        }

        private static string GetAppName(RouteData routeData)
        {
            object obj;
            return routeData.DataTokens.TryGetValue("app", out obj)
                ? obj as string
                : null;
        }

        private static string GetThemeName(RouteValueDictionary dataTokens)
        {
            return
                dataTokens.ContainsKey("theme-name")
                    ? dataTokens["theme-name"].ToString()
                    : (dataTokens["theme-name"] = CurrentRequestData.DatabaseIsInstalled &&
                                                  CurrentRequestData.SiteSettings != null &&
                                                  !string.IsNullOrWhiteSpace(CurrentRequestData.SiteSettings.ThemeName)
                        ? CurrentRequestData.SiteSettings.ThemeName
                        : string.Empty).ToString();
        }

        private class ViewLocation
        {
            protected readonly string _virtualPathFormatstring;

            public ViewLocation(string virtualPathFormatstring)
            {
                _virtualPathFormatstring = virtualPathFormatstring;
            }

            public virtual string Format(string viewName, string controllerName, string areaName, string appName,
                string themeName)
            {
                return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatstring, viewName, controllerName,
                    areaName, appName, themeName);
            }
        }

        internal class DefaultViewPageActivator : IViewPageActivator
        {
            private readonly Func<IDependencyResolver> _resolverThunk;

            public DefaultViewPageActivator()
                : this(null)
            {
            }

            public DefaultViewPageActivator(IDependencyResolver resolver)
            {
                if (resolver == null)
                {
                    _resolverThunk = () => DependencyResolver.Current;
                }
                else
                {
                    _resolverThunk = () => resolver;
                }
            }

            public object Create(ControllerContext controllerContext, Type type)
            {
                return _resolverThunk().GetService(type) ?? Activator.CreateInstance(type);
            }
        }
    }
}