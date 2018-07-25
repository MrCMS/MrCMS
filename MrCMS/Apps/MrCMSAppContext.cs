using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using MrCMS.FileProviders;

namespace MrCMS.Apps
{
    public class MrCMSAppContext
    {
        public MrCMSAppContext()
        {
            Apps = new HashSet<IMrCMSApp>();
        }

        public ISet<IMrCMSApp> Apps { get; }

        public IEnumerable<IFileProvider> ViewFileProviders =>
            Apps.Select(app => new EmbeddedViewFileProvider(app.Assembly));

        public IEnumerable<IFileProvider> ContentFileProviders =>
            Apps.Select(app => new EmbeddedContentFileProvider(app.Assembly));

        public void RegisterApp<TApp>() where TApp : IMrCMSApp, new() => Apps.Add(new TApp());
    }
}