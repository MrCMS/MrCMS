using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Bundling
{
    public interface IResourceBundler
    {
        void AddScript(string virtualPath, string url);
        void AddCss(string virtualPath, string url);
        MvcHtmlString GetScripts();
        MvcHtmlString GetCss();
    }

    public class ResourceBundler : IResourceBundler
    {
        private readonly SEOSettings _seoSettings;

        public ResourceBundler(SEOSettings seoSettings)
        {
            _seoSettings = seoSettings;
        }

        private Dictionary<string, List<ResourceData>> CssData
        {
            get
            {
                const string currentStylelist = "current.stylelist";
                if (CurrentRequestData.CurrentContext.Items[currentStylelist] == null)
                {
                    CurrentRequestData.CurrentContext.Items[currentStylelist] =
                        new Dictionary<string, List<ResourceData>>();
                }
                return CurrentRequestData.CurrentContext.Items[currentStylelist] as Dictionary<string, List<ResourceData>>;
            }
        }

        private Dictionary<string, List<ResourceData>> ScriptData
        {
            get
            {
                const string currentScriptlist = "current.scriptlist";
                if (CurrentRequestData.CurrentContext.Items[currentScriptlist] == null)
                {
                    CurrentRequestData.CurrentContext.Items[currentScriptlist] =
                        new Dictionary<string, List<ResourceData>>();
                }
                return CurrentRequestData.CurrentContext.Items[currentScriptlist] as Dictionary<string, List<ResourceData>>;
            }
        }

        public void AddScript(string virtualPath, string url)
        {
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!ScriptData.ContainsKey(virtualPath))
                ScriptData[virtualPath] = new List<ResourceData>();
            ScriptData[virtualPath].Add(new ResourceData(uri.IsAbsoluteUri, url));
        }

        public void AddCss(string virtualPath, string url)
        {
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!CssData.ContainsKey(virtualPath))
                CssData[virtualPath] = new List<ResourceData>();
            CssData[virtualPath].Add(new ResourceData(uri.IsAbsoluteUri, url));
        }

        private static readonly object s_lock = new object();
        public MvcHtmlString GetScripts()
        {
            if (_seoSettings.EnableJsBundling)
            {
                var result = new StringBuilder();
                foreach (var key in ScriptData.Keys)
                {
                    var partsToBundle = ScriptData[key]
                        .Where(x => !x.IsRemote)
                        .Select(x => x.Url)
                        .Distinct()
                        .ToArray();
                    var partsToDontBundle = ScriptData[key]
                        .Where(x => x.IsRemote)
                        .Select(x => x.Url)
                        .Distinct()
                        .ToArray();



                    if (partsToBundle.Any())
                    {
                        //IMPORTANT: Do not use bundling in web farms or Windows Azure
                        string bundleVirtualPath = GetBundleVirtualPath("~/bundles/scripts/", ".js", partsToBundle);
                        //System.Web.Optimization library does not support dynamic bundles yet.
                        //But we know how System.Web.Optimization library stores cached results.
                        //so let's clear the cache because we add new file references dynamically based on a page
                        //until it's officially supported in System.Web.Optimization we have to "workaround" it manually
                        //var cacheKey = (string)typeof(Bundle)
                        //    .GetMethod("GetCacheKey", BindingFlags.Static | BindingFlags.NonPublic)
                        //    .Invoke(null, new object[] { bundleVirtualPath });
                        //or use the code below
                        //TODO: ...but periodically ensure that cache key which we use is valid (decompile Bundle.GetCacheKey method)
                        //var cacheKey = "System.Web.Optimization.Bundle:" + bundleVirtualPath;

                        //if (_httpContext.Cache[cacheKey] != null)
                        //    _httpContext.Cache.Remove(cacheKey);

                        //create bundle
                        lock (s_lock)
                        {
                            var bundleFor = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
                            if (bundleFor == null)
                            {
                                var bundle = new ScriptBundle(bundleVirtualPath)
                                                 {
                                                     Orderer = new AsIsBundleOrderer(),
                                                     EnableFileExtensionReplacements = false
                                                 };

                                //"As is" ordering
                                //disable file extension replacements. renders scripts which were specified by a developer
                                bundle.Include(partsToBundle);
                                BundleTable.Bundles.Add(bundle);
                                //we clear ignore list because System.Web.Optimization library adds ignore patterns such as "*.min", "*.debug".
                                //we think it's bad decision and should be disabled by default
                                BundleTable.Bundles.IgnoreList.Clear();
                            }
                        }

                        //parts to bundle
                        result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                            bundleVirtualPath.Substring(1));
                        result.AppendLine();
                    }

                    //parts to do not bundle
                    foreach (var path in partsToDontBundle)
                    {
                        result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>", path);
                        result.Append(Environment.NewLine);
                    }


                }
                return MvcHtmlString.Create(result.ToString());

            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var path in ScriptData.Values.SelectMany(x => x).Select(data => data.Url).Distinct())
                {
                    result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                        path.StartsWith("~") ? path.Substring(1) : path);
                    result.Append(Environment.NewLine);
                }
                return MvcHtmlString.Create(result.ToString());
            }
        }

        public MvcHtmlString GetCss()
        {
            if (_seoSettings.EnableCssBundling)
            {
                var result = new StringBuilder();
                foreach (var key in CssData.Keys)
                {
                    var partsToBundle = CssData[key]
                        .Where(x => !x.IsRemote)
                        .Select(x => x.Url)
                        .Distinct()
                        .ToArray();
                    var partsToDontBundle = CssData[key]
                        .Where(x => x.IsRemote)
                        .Select(x => x.Url)
                        .Distinct()
                        .ToArray();



                    if (partsToBundle.Any())
                    {
                        //IMPORTANT: Do not use bundling in web farms or Windows Azure
                        string bundleVirtualPath = GetBundleVirtualPath("~/bundles/styles/", ".css", partsToBundle);
                        //System.Web.Optimization library does not support dynamic bundles yet.
                        //But we know how System.Web.Optimization library stores cached results.
                        //so let's clear the cache because we add new file references dynamically based on a page
                        //until it's officially supported in System.Web.Optimization we have to "workaround" it manually
                        //var cacheKey = (string)typeof(Bundle)
                        //    .GetMethod("GetCacheKey", BindingFlags.Static | BindingFlags.NonPublic)
                        //    .Invoke(null, new object[] { bundleVirtualPath });
                        //or use the code below
                        //TODO: ...but periodically ensure that cache key which we use is valid (decompile Bundle.GetCacheKey method)
                        //var cacheKey = "System.Web.Optimization.Bundle:" + bundleVirtualPath;

                        //if (_httpContext.Cache[cacheKey] != null)
                        //    _httpContext.Cache.Remove(cacheKey);

                        //create bundle
                        lock (s_lock)
                        {
                            var bundleFor = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
                            if (bundleFor == null)
                            {
                                var bundle = new StyleBundle(bundleVirtualPath)
                                                 {
                                                     Orderer = new AsIsBundleOrderer(),
                                                     EnableFileExtensionReplacements = false
                                                 };

                                //"As is" ordering
                                //disable file extension replacements. renders scripts which were specified by a developer
                                bundle.Include(partsToBundle);
                                BundleTable.Bundles.Add(bundle);
                                //we clear ignore list because System.Web.Optimization library adds ignore patterns such as "*.min", "*.debug".
                                //we think it's bad decision and should be disabled by default
                                BundleTable.Bundles.IgnoreList.Clear();
                            }
                        }

                        //parts to bundle
                        result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                            bundleVirtualPath.Substring(1));
                        result.AppendLine();
                    }

                    //parts to do not bundle
                    foreach (var path in partsToDontBundle)
                    {
                        result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", path);
                        result.Append(Environment.NewLine);
                    }
                }
                return MvcHtmlString.Create(result.ToString());
            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var path in ScriptData.Values.SelectMany(x => x).Select(x => x.Url).Distinct())
                {
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                        path.StartsWith("~") ? path.Substring(1) : path);
                    result.Append(Environment.NewLine);
                }
                return MvcHtmlString.Create(result.ToString());
            }
        }

        protected virtual string GetBundleVirtualPath(string prefix, string postfix, string[] parts)
        {
            if (parts == null || parts.Length == 0)
                throw new ArgumentException("parts");

            //calculate hash
            var hash = "";
            using (SHA256 sha = new SHA256Managed())
            {
                // string concatenation
                var hashInput = "";
                foreach (var part in parts)
                {
                    hashInput += part;
                    hashInput += ",";
                }

                byte[] input = sha.ComputeHash(Encoding.Unicode.GetBytes(hashInput));
                hash = HttpServerUtility.UrlTokenEncode(input);
            }
            //ensure only valid chars
            hash = FileService.RemoveInvalidUrlCharacters(hash);

            var sb = new StringBuilder(prefix);
            sb.Append(hash);
            sb.Append(postfix);
            return sb.ToString();
        }
    }

    public class BundleData
    {
        public BundleData(string name, IEnumerable<string> files)
        {
            Name = name;
            Files = files;
        }

        public string Name { get; set; }
        public IEnumerable<string> Files { get; set; }
    }

    public class ResourceData
    {
        public ResourceData(bool isRemote, string url)
        {
            IsRemote = isRemote;
            Url = url;
        }

        public bool IsRemote { get; set; }
        public string Url { get; set; }
    }

    public partial class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<System.IO.FileInfo> OrderFiles(BundleContext context, IEnumerable<System.IO.FileInfo> files)
        {
            return files;
        }
    }
}