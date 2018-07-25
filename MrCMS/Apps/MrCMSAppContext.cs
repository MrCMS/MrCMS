using System;
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
            Apps.Select(app => new EmbeddedContentFileProvider(app.Assembly, app.ContentPrefix));

        public void RegisterApp<TApp>(Action<MrCMSAppOptions> options = null) where TApp : IMrCMSApp, new()
        {
            var appOptions = new MrCMSAppOptions();
            options?.Invoke(appOptions);
            var app = new TApp();
            if (!string.IsNullOrWhiteSpace(appOptions.ContentPrefix))
                app.ContentPrefix = appOptions.ContentPrefix;
            Apps.Add(app);
        }
    }
}