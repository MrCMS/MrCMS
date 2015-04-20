using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MrCMS.Website.Optimization
{
    public class ResourceBundler : IResourceBundler
    {
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
                return
                    CurrentRequestData.CurrentContext.Items[currentStylelist] as Dictionary<string, List<ResourceData>>;
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
                return
                    CurrentRequestData.CurrentContext.Items[currentScriptlist] as Dictionary<string, List<ResourceData>>;
            }
        }

        public void AddScript(string virtualPath, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!ScriptData.ContainsKey(virtualPath))
                ScriptData[virtualPath] = new List<ResourceData>();
            ScriptData[virtualPath].Add(ResourceData.Get(uri.IsAbsoluteUri, url));
        }

        public void AddCss(string virtualPath, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!CssData.ContainsKey(virtualPath))
                CssData[virtualPath] = new List<ResourceData>();
            CssData[virtualPath].Add(ResourceData.Get(uri.IsAbsoluteUri, url));
        }

        public void GetScripts(ViewContext viewContext)
        {
            foreach (string path in ScriptData.Values.SelectMany(x => x).Select(data => data.Url).Distinct())
            {
                viewContext.Writer.Write("<script src=\"{0}\" type=\"text/javascript\"></script>",
                    path.StartsWith("~") ? path.Substring(1) : path);
            }
        }

        public void GetCss(ViewContext viewContext)
        {
            foreach (string path in CssData.Values.SelectMany(x => x).Select(x => x.Url).Distinct())
            {
                viewContext.Writer.Write("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                    path.StartsWith("~") ? path.Substring(1) : path);
            }
        }
    }
}