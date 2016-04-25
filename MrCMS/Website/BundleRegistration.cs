using System.Web.Optimization;
using MrCMS.Settings;
using MrCMS.Website.Optimization;
using Ninject;

namespace MrCMS.Website
{
    public static class BundleRegistration
    {
        public static void Register(IKernel kernel)
        {
            if (!CurrentRequestData.DatabaseIsInstalled)
                return;

            foreach (var bundle in kernel.GetAll<IStylesheetBundle>())
            {
                var styleBundle = new StyleBundle(bundle.Url);
                foreach (var file in bundle.Files)
                {
                    styleBundle.Include(file);
                }
                BundleTable.Bundles.Add(styleBundle);
            }

            foreach (var bundle in kernel.GetAll<IScriptBundle>())
            {
                var scriptBundle = new ScriptBundle(bundle.Url);
                foreach (var file in bundle.Files)
                {
                    scriptBundle.Include(file);
                }
                BundleTable.Bundles.Add(scriptBundle);
            }
            BundleTable.EnableOptimizations = kernel.Get<BundlingSettings>().EnableOptimisations;
        }
    }
}