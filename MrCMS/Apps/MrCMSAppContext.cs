using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using MrCMS.FileProviders;
using MrCMS.Helpers;
using MrCMS.Website;
using MrCMS.Website.CMS;
using NHibernate.Cfg;

namespace MrCMS.Apps
{
    public class MrCMSAppContext
    {
        public MrCMSAppContext()
        {
            Apps = new HashSet<IMrCMSApp>();
            Types = new Dictionary<Type, IMrCMSApp>();
        }

        public ISet<IMrCMSApp> Apps { get; }
        public IDictionary<Type, IMrCMSApp> Types { get; }

        public IEnumerable<IFileProvider> ViewFileProviders =>
            Apps.Select(app => new EmbeddedViewFileProvider(app.Assembly, app.ViewPrefix));

        public IEnumerable<IFileProvider> ContentFileProviders =>
            Apps.Select(app => new EmbeddedContentFileProvider(app.Assembly, app.ContentPrefix));

        public IEnumerable<Type> DbConventions => Apps.SelectMany(app => app.Conventions);
        public IEnumerable<Type> DbBaseTypes => Apps.SelectMany(app => app.BaseTypes);

        public IDictionary<Type, string> SignalRHubs =>
            Apps.SelectMany(app => app.SignalRHubs).ToDictionary(x => x.Key, x => x.Value); 

        // TODO: get middleware per app
        public IEnumerable<RegistrationInfo> Registrations => Apps.SelectMany(app => app.Registrations);

        public string AppSummary
        {
            get { return string.Join(", ", Apps.Select(app => $"{app.Name}: {app.Version}")); }

        }

        public void RegisterApp<TApp>(Action<MrCMSAppOptions> options = null) where TApp : IMrCMSApp, new()
        {
            var appOptions = new MrCMSAppOptions();
            options?.Invoke(appOptions);
            var app = new TApp();
            if (!string.IsNullOrWhiteSpace(appOptions.ContentPrefix))
                app.ContentPrefix = appOptions.ContentPrefix;
            Apps.Add(app);

            // register apps
            app.Assembly.GetTypes().ForEach(type => Types[type] = app);
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