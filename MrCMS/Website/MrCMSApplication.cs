using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.DbConfiguration.Caches.Redis;
using MrCMS.Messages;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using MrCMS.Website.Caching;
using MrCMS.Website.Filters;
using MrCMS.Website.Routing;
using Ninject;
using StackExchange.Profiling;

namespace MrCMS.Website
{
    public abstract class MrCMSApplication : HttpApplication
    {
        public const string AssemblyVersion = "0.6.0.0";
        public const string AssemblyFileVersion = "0.6.0.0";
        private const string CachedMissingItemKey = "cached-missing-item";


        private static IOnEndRequestExecutor OnEndRequestExecutor => MrCMSKernel.Kernel.Get<IOnEndRequestExecutor>();

        protected void Application_Start()
        {
            SetModelBinders();
            SetViewEngines();

            MrCMSApp.RegisterAllApps();
            AreaRegistration.RegisterAllAreas(MrCMSKernel.Kernel);
            MrCMSRouteRegistration.Register(RouteTable.Routes);

            RegisterServices(MrCMSKernel.Kernel);
            MrCMSApp.RegisterAllServices(MrCMSKernel.Kernel);

            LegacySettingMigrator.MigrateSettings(MrCMSKernel.Kernel);
            LegacyTemplateMigrator.MigrateTemplates(MrCMSKernel.Kernel);

            BundleRegistration.Register(MrCMSKernel.Kernel);

            ControllerBuilder.Current.SetControllerFactory(new MrCMSControllerFactory());

            FilterProviders.Providers.Insert(0, new GlobalFilterProvider(MrCMSKernel.Kernel,
                typeof(HoneypotFilter),
                typeof(GoogleRecaptchaFilter),
                typeof(DoNotCacheFilter)
            ));


            ModelMetadataProviders.Current = new MrCMSMetadataProvider(MrCMSKernel.Kernel);

            ImagePluginInstaller.Install();

            MiniProfiler.Settings.Results_Authorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;
            MiniProfiler.Settings.Results_List_Authorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;


            OnApplicationStart();
        }

        protected void Application_End()
        {
            if (RedisCacheInitializer.Initialized)
                RedisCacheInitializer.Dispose();
        }


        protected virtual void SetViewEngines()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Insert(0, new MrCMSRazorViewEngine());
        }

        protected virtual void OnApplicationStart()
        {
        }

        protected virtual void SetModelBinders()
        {
            ModelBinders.Binders.DefaultBinder = new MrCMSDefaultModelBinder(MrCMSKernel.Kernel);
            ModelBinders.Binders.Add(typeof(DateTime), new CultureAwareDateBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableCultureAwareDateBinder());
        }


        public override void Init()
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                BeginRequest += (sender, args) =>
                {
                    if (IsCachedMissingFileRequest())
                        return;
                    RequestInitializer.Initialize(Request);

                    OnBeginRequest(sender, args);
                };
                AuthenticateRequest += (sender, args) =>
                {
                    if (!Context.Items.Contains(CachedMissingItemKey) && !RequestInitializer.IsFileRequest(Request.Url))
                        RequestAuthenticator.Authenticate(Request);
                    OnAuthenticateRequest(sender, args);
                };
                EndRequest += (sender, args) =>
                {
                    if (Context.Items.Contains(CachedMissingItemKey))
                        return;

                    OnEndRequestExecutor.ExecuteTasks(CurrentRequestData.OnEndRequest);

                    OnEndRequest(sender, args);

                    MiniProfiler.Stop();
                };
            }
            else
            {
                EndRequest += (sender, args) => OnEndRequestExecutor.ExecuteTasks(CurrentRequestData.OnEndRequest);
            }
        }

        protected virtual void OnEndRequest(object sender, EventArgs args)
        {
        }

        protected virtual void OnAuthenticateRequest(object sender, EventArgs args)
        {
        }

        protected virtual void OnBeginRequest(object sender, EventArgs args)
        {
        }

        private bool IsCachedMissingFileRequest()
        {
            var missingFile =
                Convert.ToString(
                    Get<ICacheWrapper>()[FileNotFoundHandler.GetMissingFileCacheKey(new HttpRequestWrapper(Request))]);
            if (!string.IsNullOrWhiteSpace(missingFile))
            {
                Context.Items[CachedMissingItemKey] = true;
                Context.Response.Clear();
                Context.Response.StatusCode = 404;
                Context.Response.TrySkipIisCustomErrors = true;
                Context.Response.Write(missingFile);
                Context.ApplicationInstance.CompleteRequest();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Load your modules or register your non-app specific services here
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        protected virtual void RegisterServices(IKernel kernel)
        {
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return MrCMSKernel.Kernel.GetAll<T>();
        }

        public static T Get<T>()
        {
            return MrCMSKernel.Kernel.Get<T>();
        }

        public static object Get(Type type)
        {
            return MrCMSKernel.Kernel.Get(type);
        }
    }
}