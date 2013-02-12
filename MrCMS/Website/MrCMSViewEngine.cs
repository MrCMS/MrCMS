using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace MrCMS.Website
{
    public class MrCMSRazorViewEngine : MrCMSVirtualPathProviderViewEngine
    {
        internal static readonly string ViewStartFileName = "_ViewStart";
        private IViewPageActivator _viewPageActivator;

        public MrCMSRazorViewEngine()
            : this(null)
        {
        }

        public MrCMSRazorViewEngine(IViewPageActivator viewPageActivator)
        {
            _viewPageActivator = viewPageActivator;
            AppViewLocationFormats = new[] {
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.cshtml", 
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.vbhtml" ,
                "~/Apps/{3}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Views/{1}/{0}.vbhtml", 
                "~/Apps/{3}/Views/Shared/{0}.cshtml", 
                "~/Apps/{3}/Views/Shared/{0}.vbhtml",
                "~/Apps/{3}/Views/Pages/{0}.cshtml",
                "~/Apps/{3}/Views/Pages/{0}.vbhtml"
            };
            AppMasterLocationFormats = new[] { 
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.cshtml", 
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.vbhtml" ,
                "~/Apps/{3}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Views/{1}/{0}.vbhtml", 
                "~/Apps/{3}/Views/Shared/{0}.cshtml", 
                "~/Apps/{3}/Views/Shared/{0}.vbhtml"
            };
            AppPartialViewLocationFormats = new[] { 
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.cshtml", 
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.vbhtml" ,
                "~/Apps/{3}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Views/{1}/{0}.vbhtml", 
                "~/Apps/{3}/Views/Shared/{0}.cshtml", 
                "~/Apps/{3}/Views/Shared/{0}.vbhtml",
                "~/Apps/{3}/Views/Widgets/{0}.cshtml",
                "~/Apps/{3}/Views/Widgets/{0}.vbhtml"
            };



            AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                "~/Areas/{2}/Views/Shared/{0}.vbhtml" 
            };
            AreaMasterLocationFormats = new[] { 
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml" 
            };
            AreaPartialViewLocationFormats = new[] { 
                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            ViewLocationFormats = new[] { 
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml", 
                "~/Views/Shared/{0}.cshtml", 
                "~/Views/Shared/{0}.vbhtml",
                "~/Views/Pages/{0}.cshtml",
                "~/Views/Pages/{0}.vbhtml"
            };
            MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml", 
                "~/Views/Shared/{0}.vbhtml"
            };
            PartialViewLocationFormats = new[] { 
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml", 
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml",
                "~/Views/Widgets/{0}.cshtml",
                "~/Views/Widgets/{0}.vbhtml"
            };

            FileExtensions = new[] {
                "cshtml", 
                "vbhtml", 
            };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new RazorView(controllerContext, partialPath,
                                 layoutPath: null, runViewStartPages: false, viewStartFileExtensions: FileExtensions, viewPageActivator: ViewPageActivator);
        }

        private IViewPageActivator ViewPageActivator
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
        }

        internal class DefaultViewPageActivator : IViewPageActivator
        {
            Func<IDependencyResolver> _resolverThunk;

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

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var view = new RazorView(controllerContext, viewPath,
                                     layoutPath: masterPath, runViewStartPages: true, viewStartFileExtensions: FileExtensions, viewPageActivator: ViewPageActivator);
            return view;
        }
    }

    public abstract class MrCMSVirtualPathProviderViewEngine : IViewEngine
    {
        // format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:"
        private const string _cacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:";
        private const string _cacheKeyPrefix_Master = "Master";
        private const string _cacheKeyPrefix_Partial = "Partial";
        private const string _cacheKeyPrefix_View = "View";
        private static readonly string[] _emptyLocations = new string[0];

        private VirtualPathProvider _vpp;
        internal Func<string, string> GetExtensionThunk = VirtualPathUtility.GetExtension;

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] AppMasterLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] AreaMasterLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] AppPartialViewLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] AreaPartialViewLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] AppViewLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] AreaViewLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] FileExtensions
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] MasterLocationFormats
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] PartialViewLocationFormats
        {
            get;
            set;
        }

        public IViewLocationCache ViewLocationCache
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ViewLocationFormats
        {
            get;
            set;
        }

        protected VirtualPathProvider VirtualPathProvider
        {
            get
            {
                if (_vpp == null)
                {
                    _vpp = HostingEnvironment.VirtualPathProvider;
                }
                return _vpp;
            }
            set
            {
                _vpp = value;
            }
        }

        protected MrCMSVirtualPathProviderViewEngine()
        {
            if (HttpContext.Current == null || HttpContext.Current.IsDebuggingEnabled)
            {
                ViewLocationCache = DefaultViewLocationCache.Null;
            }
            else
            {
                ViewLocationCache = new DefaultViewLocationCache();
            }
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string areaName)
        {
            return String.Format(CultureInfo.InvariantCulture, _cacheKeyFormat,
                GetType().AssemblyQualifiedName, prefix, name, controllerName, areaName);
        }

        protected abstract IView CreatePartialView(ControllerContext controllerContext, string partialPath);

        protected abstract IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath);

        protected virtual bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return VirtualPathProvider.FileExists(virtualPath);
        }

        public virtual ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (String.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException("Argument null or empty", "partialViewName");
            }

            string[] searched;
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string partialPath = GetPath(controllerContext, PartialViewLocationFormats, AreaPartialViewLocationFormats, AppPartialViewLocationFormats, "PartialViewLocationFormats", partialViewName, controllerName, _cacheKeyPrefix_Partial, useCache, out searched);

            if (String.IsNullOrEmpty(partialPath))
            {
                return new ViewEngineResult(searched);
            }

            return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
        }

        public virtual ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (String.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException("Argument null or empty", "viewName");
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string viewPath = GetPath(controllerContext, ViewLocationFormats, AreaViewLocationFormats, AppViewLocationFormats, "ViewLocationFormats", viewName, controllerName, _cacheKeyPrefix_View, useCache, out viewLocationsSearched);
            string masterPath = GetPath(controllerContext, MasterLocationFormats, AreaMasterLocationFormats, AppMasterLocationFormats, "MasterLocationFormats", masterName, controllerName, _cacheKeyPrefix_Master, useCache, out masterLocationsSearched);

            if (String.IsNullOrEmpty(viewPath) || (String.IsNullOrEmpty(masterPath) && !String.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
            }

            return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations, string[] appLocations, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = _emptyLocations;

            if (String.IsNullOrEmpty(name))
            {
                return String.Empty;
            }

            string areaName = GetAreaName(controllerContext.RouteData);
            string appName = GetAppName(controllerContext.RouteData);
            bool usingAreas = !String.IsNullOrEmpty(areaName);
            bool usingApps = !String.IsNullOrEmpty(appName);
            List<ViewLocation> viewLocations = GetViewLocations(locations, (usingAreas) ? areaLocations : null,
                                                                (usingApps) ? appLocations : null);

            if (viewLocations.Count == 0)
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                                  "Property {0} cannot be null or empty",
                                                                  locationsPropertyName));

            bool nameRepresentsPath = IsSpecificPath(name);
            string cacheKey = CreateCacheKey(cacheKeyPrefix, name, (nameRepresentsPath) ? String.Empty : controllerName, areaName);

            return useCache
                       ? ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey)
                       : ((nameRepresentsPath)
                              ? GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations)
                              : GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName,
                                                       appName, cacheKey, ref searchedLocations));
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name, string controllerName, string areaName, string appName, string cacheKey, ref string[] searchedLocations)
        {
            string result = String.Empty;
            searchedLocations = new string[locations.Count];

            for (int i = 0; i < locations.Count; i++)
            {
                ViewLocation location = locations[i];
                string virtualPath = location.Format(name, controllerName, areaName, appName);

                if (FileExists(controllerContext, virtualPath))
                {
                    searchedLocations = _emptyLocations;
                    result = virtualPath;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
                    break;
                }

                searchedLocations[i] = virtualPath;
            }

            return result;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string result = name;

            if (!(FilePathIsSupported(name) && FileExists(controllerContext, name)))
            {
                result = String.Empty;
                searchedLocations = new[] { name };
            }

            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        private bool FilePathIsSupported(string virtualPath)
        {
            if (FileExtensions == null)
            {
                // legacy behavior for custom ViewEngine that might not set the FileExtensions property
                return true;
            }

            // get rid of the '.' because the FileExtensions property expects extensions withouth a dot.
            string extension = GetExtensionThunk(virtualPath).TrimStart('.');
            return FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private static List<ViewLocation> GetViewLocations(string[] viewLocationFormats, string[] areaViewLocationFormats, string[] appViewLocationFormats)
        {
            List<ViewLocation> allLocations = new List<ViewLocation>();

            if (appViewLocationFormats != null)
            {
                allLocations.AddRange(appViewLocationFormats.Select(appViewLocationFormat => new AppAwareViewLocation(appViewLocationFormat)).Cast<ViewLocation>());
            }

            if (areaViewLocationFormats != null)
            {
                allLocations.AddRange(areaViewLocationFormats.Select(areaViewLocationFormat => new AreaAwareViewLocation(areaViewLocationFormat)).Cast<ViewLocation>());
            }

            if (viewLocationFormats != null)
            {
                allLocations.AddRange(viewLocationFormats.Select(viewLocationFormat => new ViewLocation(viewLocationFormat)));
            }

            return allLocations;
        }

        private static bool IsSpecificPath(string name)
        {
            char c = name[0];
            return (c == '~' || c == '/');
        }

        public virtual void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var disposable = view as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        private class ViewLocation
        {
            protected readonly string _virtualPathFormatString;

            public ViewLocation(string virtualPathFormatString)
            {
                _virtualPathFormatString = virtualPathFormatString;
            }

            public virtual string Format(string viewName, string controllerName, string areaName, string appName)
            {
                return String.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName);
            }

        }

        private class AreaAwareViewLocation : ViewLocation
        {

            public AreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string appName)
            {
                return String.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, areaName);
            }

        }

        private class AppAwareViewLocation : ViewLocation
        {

            public AppAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string appName)
            {
                return String.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName,
                                     areaName, appName);
            }

        }

        private static string GetAreaName(RouteBase route)
        {
            var routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
                return routeWithArea.Area;
            var route1 = route as Route;
            return route1 != null && route1.DataTokens != null
                       ? route1.DataTokens["area"] as string
                       : (string)null;
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
    }
}