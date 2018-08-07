using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using MrCMS.FileProviders;
using MrCMS.Helpers;
using NHibernate.Cfg;

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
            Apps.Select(app => new EmbeddedViewFileProvider(app.Assembly, app.ViewPrefix));

        public IEnumerable<IFileProvider> ContentFileProviders =>
            Apps.Select(app => new EmbeddedContentFileProvider(app.Assembly, app.ContentPrefix));

        public IEnumerable<Type> DbConventions => Apps.SelectMany(app => app.Conventions);

        public void RegisterApp<TApp>(Action<MrCMSAppOptions> options = null) where TApp : IMrCMSApp, new()
        {
            var appOptions = new MrCMSAppOptions();
            options?.Invoke(appOptions);
            var app = new TApp();
            if (!string.IsNullOrWhiteSpace(appOptions.ContentPrefix))
                app.ContentPrefix = appOptions.ContentPrefix;
            Apps.Add(app);
        }

        public void SetupMvcOptions(MvcOptions options)
        {
            Apps.ForEach(app => app.SetupMvcOptions(options));
        }

        public void ConfigureAutomapper(IMapperConfigurationExpression expression)
        {
            Apps.ForEach(app => app.ConfigureAutomapper(expression));
        }

        public void AppendConfiguration(Configuration configuration)
        {
            Apps.ForEach(app => app.AppendConfiguration(configuration));
        }
    }
}