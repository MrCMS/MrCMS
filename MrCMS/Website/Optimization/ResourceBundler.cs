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

namespace MrCMS.Website.Optimization
{
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

            var result = new StringBuilder();
            if (_seoSettings.EnableJsBundling)
            {
                foreach (var key in ScriptData.Keys)
                {
                    var partsToBundle = ScriptData[key].Where(x => !x.IsRemote).Select(x => x.Url).Distinct().ToArray();

                    var partsToNotBundle =
                        ScriptData[key].Where(x => x.IsRemote).Select(x => x.Url).Distinct().ToArray();

                    if (partsToBundle.Any())
                    {
                        string bundleVirtualPath = GetBundleVirtualPath("~/bundles/scripts/", ".js", partsToBundle);

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
                                bundle.Include(partsToBundle);
                                BundleTable.Bundles.Add(bundle);
                                BundleTable.Bundles.IgnoreList.Clear();
                            }
                        }

                        result.AppendLine(string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                                        bundleVirtualPath.Substring(1)));
                    }

                    foreach (var path in partsToNotBundle)
                        result.AppendLine(string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", path));
                }
                return MvcHtmlString.Create(result.ToString());

            }
            foreach (var path in ScriptData.Values.SelectMany(x => x).Select(data => data.Url).Distinct())
            {
                result.AppendLine(string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                                path.StartsWith("~") ? path.Substring(1) : path));
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public MvcHtmlString GetCss()
        {
            var result = new StringBuilder();
            if (_seoSettings.EnableCssBundling)
            {
                foreach (var key in CssData.Keys)
                {
                    var partsToBundle = CssData[key].Where(x => !x.IsRemote).Select(x => x.Url).Distinct().ToArray();
                    var partsToDontBundle = CssData[key].Where(x => x.IsRemote).Select(x => x.Url).Distinct().ToArray();

                    if (partsToBundle.Any())
                    {
                        string bundleVirtualPath = GetBundleVirtualPath("~/bundles/styles/", ".css", partsToBundle);
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
                                bundle.Include(partsToBundle);
                                BundleTable.Bundles.Add(bundle);
                                BundleTable.Bundles.IgnoreList.Clear();
                            }
                        }

                        result.AppendLine(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                                        bundleVirtualPath.Substring(1)));
                    }

                    foreach (var path in partsToDontBundle)
                        result.AppendLine(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                                        path));
                }
                return MvcHtmlString.Create(result.ToString());
            }

            foreach (var path in CssData.Values.SelectMany(x => x).Select(x => x.Url).Distinct())
                result.AppendLine(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                                                path.StartsWith("~") ? path.Substring(1) : path));
            return MvcHtmlString.Create(result.ToString());
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
}