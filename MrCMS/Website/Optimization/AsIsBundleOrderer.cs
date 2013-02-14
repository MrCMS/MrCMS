using System.Collections.Generic;
using System.Web.Optimization;

namespace MrCMS.Website.Optimization
{
    public partial class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<System.IO.FileInfo> OrderFiles(BundleContext context, IEnumerable<System.IO.FileInfo> files)
        {
            return files;
        }
    }
}