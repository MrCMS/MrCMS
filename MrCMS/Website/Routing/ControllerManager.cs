using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website.Binders;

namespace MrCMS.Website.Routing
{
    public class ControllerManager : IControllerManager
    {
        private static readonly IControllerFactory DefaultControllerFactory =
            ControllerBuilder.Current.GetControllerFactory();

        private readonly IUserUIPermissionsService _userUIPermissionsService;
        public Func<Document, DocumentMetadata> GetMetadata = document => document.GetMetadata();

        public ControllerManager(IUserUIPermissionsService userUIPermissionsService)
        {
            _userUIPermissionsService = userUIPermissionsService;
        }

        public IControllerFactory OverridenControllerFactory { get; set; }

        public IControllerFactory ControllerFactory
        {
            get { return OverridenControllerFactory ?? DefaultControllerFactory; }
        }

        public void SetFormData(Webpage webpage, Controller controller, NameValueCollection form)
        {
            if (form != null)
            {
                var formCollection = new FormCollection(form);
                DocumentMetadata metadata = GetMetadata(webpage);
                if (metadata != null && metadata.PostTypes != null && metadata.PostTypes.Any())
                {
                    foreach (Type type in metadata.PostTypes)
                    {
                        var modelBinder = ModelBinders.Binders.GetBinder(type) as MrCMSDefaultModelBinder;
                        if (modelBinder != null)
                        {
                            var modelBindingContext = new ModelBindingContext
                            {
                                ValueProvider =
                                    formCollection,
                                ModelMetadata =
                                    ModelMetadataProviders.Current.GetMetadataForType(
                                        () =>
                                            modelBinder.GetModelFromSession(
                                                controller.ControllerContext,
                                                string.Empty, type), type)
                            };

                            object model = modelBinder.BindModel(controller.ControllerContext, modelBindingContext);
                            controller.RouteData.Values[type.Name.ToLower()] = model;
                        }
                    }
                }
                else
                {
                    controller.RouteData.Values["form"] = formCollection;
                }
            }
        }

        public string GetActionName(Webpage webpage, string httpMethod)
        {
            if (webpage == null)
                return null;

            if (!webpage.Published && !_userUIPermissionsService.IsCurrentUserAllowed(webpage))
                return null;

            DocumentMetadata metadata = GetMetadata(webpage);

            if (metadata == null) return null;

            switch (httpMethod)
            {
                case "GET":
                case "HEAD":
                    return metadata.WebGetAction;
                case "POST":
                    return metadata.WebPostAction;
                default:
                    return null;
            }
        }

        public Controller GetController(RequestContext requestContext, Webpage webpage, string httpMethod)
        {
            string controllerName = GetControllerName(webpage, httpMethod);

            var controller = ControllerFactory.CreateController(requestContext, controllerName) as Controller;

            controller.ControllerContext = new ControllerContext(requestContext, controller)
            {
                RouteData = requestContext.RouteData
            };

            var routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary["controller"] = controllerName;
            routeValueDictionary["action"] = GetActionName(webpage, httpMethod);
            routeValueDictionary["page"] = webpage;
            controller.RouteData.Values.Merge(routeValueDictionary);
            controller.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[webpage.GetType()];

            return controller;
        }

        public string GetControllerName(Webpage webpage, string httpMethod)
        {
            if (webpage == null)
                return null;

            if (!webpage.Published && !_userUIPermissionsService.IsCurrentUserAllowed(webpage))
                return null;

            DocumentMetadata metadata = GetMetadata(webpage);

            if (metadata == null) return null;

            string controllerName;

            switch (httpMethod)
            {
                case "GET":
                case "HEAD":
                    controllerName = metadata.WebGetController;
                    break;
                case "POST":
                    controllerName = metadata.WebPostController;
                    break;
                default:
                    return null;
            }

            return controllerName;
        }
    }
}