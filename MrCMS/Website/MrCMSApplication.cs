using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Tasks;
using MrCMS.Website.Binders;
using MrCMS.Website.Caching;
using MrCMS.Website.Filters;
using MrCMS.Website.Routing;
using NHibernate;
using Ninject;
using StackExchange.Profiling;

namespace MrCMS.Website
{
    public abstract class MrCMSApplication : HttpApplication
    {
        public const string AssemblyVersion = "0.5.0.0";
        public const string AssemblyFileVersion = "0.5.0.0";
        private const string CachedMissingItemKey = "cached-missing-item";


        private static IOnEndRequestExecutor OnEndRequestExecutor
        {
            get { return MrCMSKernel.Kernel.Get<IOnEndRequestExecutor>(); }
        }

        protected void Application_Start()
        {
            MrCMSApp.RegisterAllApps();
            AreaRegistration.RegisterAllAreas(MrCMSKernel.Kernel);
            MrCMSRouteRegistration.Register(RouteTable.Routes);

            RegisterServices(MrCMSKernel.Kernel);
            MrCMSApp.RegisterAllServices(MrCMSKernel.Kernel);

            SetModelBinders();

            SetViewEngines();

            BundleRegistration.Register(MrCMSKernel.Kernel);

            ControllerBuilder.Current.SetControllerFactory(new MrCMSControllerFactory());

            GlobalFilters.Filters.Add(new HoneypotFilterAttribute());

            ModelMetadataProviders.Current = new MrCMSMetadataProvider(MrCMSKernel.Kernel);

            ImagePluginInstaller.Install();

            MiniProfiler.Settings.Results_Authorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;
            MiniProfiler.Settings.Results_List_Authorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;

            StartTasksRunning();

            OnApplicationStart();
        }


        protected virtual void SetViewEngines()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Insert(0, new MrCMSRazorViewEngine());
        }

        private void StartTasksRunning()
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                TaskExecutionQueuer.Initialize(Get<ISession>().QueryOver<Site>().Cacheable().List());
            }
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
                    {
                        RequestAuthenticator.Authenticate(Request);
                    }
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
            object o = Get<ICacheWrapper>()[FileNotFoundHandler.GetMissingFileCacheKey(new HttpRequestWrapper(Request))];
            if (o != null)
            {
                Context.Items[CachedMissingItemKey] = true;
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