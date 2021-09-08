using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Themes;
using MrCMS.Website.CMS;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace MrCMS.Apps
{
    public class MrCMSAppContext
    {
        public MrCMSAppContext()
        {
            Apps = new HashSet<IMrCMSApp>();
            Themes = new HashSet<IMrCMSTheme>();
            DatabaseProviders = new HashSet<Type>();
            Types = new Dictionary<Type, IMrCMSApp>();
        }

        public ISet<IMrCMSApp> Apps { get; }
        public ISet<IMrCMSTheme> Themes { get; }
        public ISet<Type> DatabaseProviders { get; }
        public IDictionary<Type, IMrCMSApp> Types { get; }

        public IEnumerable<Type> DbConventions => Apps.SelectMany(app => app.Conventions);
        public IEnumerable<Type> DbBaseTypes => Apps.SelectMany(app => app.BaseTypes);

        public IDictionary<Type, string> SignalRHubs =>
            Apps.SelectMany(app => app.SignalRHubs).ToDictionary(x => x.Key, x => x.Value);

        public IEnumerable<ApplicationRegistrationInfo> Registrations => Apps.SelectMany(app => app.Registrations);

        public IEnumerable<EndpointRegistrationInfo> EndpointRegistrations =>
            Apps.SelectMany(app => app.EndpointRegistrations);

        public string AppSummary => string.Join(", ", Apps.Select(app => $"{app.Name}: {app.Version}"));

        public void RegisterApp<TApp>(Action<MrCMSAppContextOptions> options = null) where TApp : IMrCMSApp, new()
        {
            var appOptions = new MrCMSAppContextOptions();
            options?.Invoke(appOptions);
            var app = new TApp();
            Apps.Add(app);

            // register apps
            app.Assembly.GetTypes().ForEach(type => Types[type] = app);
        }

        public void RegisterTheme<TTheme>(Action<MrCMSAppContextOptions> options = null)
            where TTheme : IMrCMSTheme, new()
        {
            var appOptions = new MrCMSAppContextOptions();
            options?.Invoke(appOptions);
            var theme = new TTheme();

            Themes.Add(theme);
        }

        public void RegisterDatabaseProvider<TProvider>()
            where TProvider : IDatabaseProvider
        {
            DatabaseProviders.Add(typeof(TProvider));
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

        public void ConfigureAuthorization(AuthorizationOptions options)
        {
            Apps.ForEach(app => app.ConfigureAuthorization(options));
        }
    }
}