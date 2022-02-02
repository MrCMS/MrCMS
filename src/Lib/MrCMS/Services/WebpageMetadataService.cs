using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class WebpageMetadataService : IWebpageMetadataService
    {
        private readonly IServiceProvider _serviceProvider;

        public WebpageMetadataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<WebpageMetadataInfo> _webpageMetadataInfos;
        private List<WebpageMetadata> _webpageMetadata;

        public IEnumerable<WebpageMetadata> WebpageMetadata
        {
            get { return GetWebpageMetadata().Where(x => x.Type != null && x.Type.IsSubclassOf(typeof(Webpage))); }
        }

        public List<WebpageMetadata> GetWebpageMetadata()
        {
            return _webpageMetadata ??= DocumentMetadataInfos.Select(GetMetadata).ToList();
        }

        public List<WebpageMetadataInfo> DocumentMetadataInfos => _webpageMetadataInfos ??= GetDocumentMetadataInfo();

        public WebpageMetadata GetDocumentMetadata(IHtmlHelper helper, int id)
        {
            var webpage = helper.GetRequiredService<ISession>().Get<Webpage>(id);

            return GetMetadata(webpage);
        }

        private List<WebpageMetadataInfo> GetDocumentMetadataInfo()
        {
            var list = new List<WebpageMetadataInfo>();
            var allMaps = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(DocumentMetadataMap<>));

            foreach (
                Type type in
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var types = allMaps.FindAll(
                    x => typeof(DocumentMetadataMap<>).MakeGenericType(type).IsAssignableFrom(x));
                if (types.Any())
                {
                    var definition = _serviceProvider.GetRequiredService(types.First()) as IGetDocumentMetadataInfo;
                    list.Add(definition.Metadata);
                }
                else
                {
                    var definition =
                        _serviceProvider.GetRequiredService(typeof(DefaultDocumentMetadata<>).MakeGenericType(type)) as
                            IGetDocumentMetadataInfo;
                    list.Add(definition.Metadata);
                }
            }

            return list.OrderBy(x => x.DisplayOrder).ToList();
        }

        private WebpageMetadata GetMetadata(WebpageMetadataInfo info)
        {
            var validChildrenTypes = new List<Type>();
            switch (info.ChildrenListType)
            {
                case ChildrenListType.BlackList:
                    validChildrenTypes.AddRange(
                        DocumentMetadataInfos.Where(
                                metadata => !info.ChildrenList.Contains(metadata.Type) && !metadata.AutoBlacklist)
                            .Select(metadata => metadata.Type));
                    break;
                case ChildrenListType.WhiteList:
                    validChildrenTypes.AddRange(info.ChildrenList);
                    break;
            }

            return new WebpageMetadata(
                info.Name,
                info.IconClass,
                info.WebGetController,
                info.WebGetAction,
                info.WebPostController,
                info.WebPostAction,
                info.WebGetControllerUnauthorized,
                info.WebGetActionUnauthorized,
                info.WebPostControllerUnauthorized,
                info.WebPostActionUnauthorized,
                info.WebGetControllerForbidden,
                info.WebGetActionForbidden,
                info.WebPostControllerForbidden,
                info.WebPostActionForbidden,
                info.MaxChildNodes,
                info.Sortable,
                info.SortBy,
                info.DisplayOrder,
                info.Type,
                info.PostTypes,
                info.ChildrenListType,
                info.ChildrenList,
                info.AutoBlacklist,
                info.RequiresParent,
                info.DefaultLayoutName,
                info.EditPartialView,
                info.ShowChildrenInAdminNav,
                info.ChildrenMaintainHierarchy,
                info.HasBodyContent,
                info.RevealInNavigation,
                validChildrenTypes
            );
        }

        public Type GetTypeByName(string name)
        {
            return GetWebpageMetadata().Where(x => x.TypeName == name).Select(x => x.Type).FirstOrDefault();
        }

        public string GetIconClass(Webpage webpage)
        {
            WebpageMetadata webpageTypeDefinition =
                GetWebpageMetadata().FirstOrDefault(
                    x => webpage.GetType().Name.Equals(x.TypeName, StringComparison.OrdinalIgnoreCase));

            return webpageTypeDefinition.IconClass;
        }

        public WebpageMetadata GetMetadata(Type type)
        {
            return GetWebpageMetadata().FirstOrDefault(x => x.Type.Name == type.Name);
        }

        public WebpageMetadata GetMetadata(Webpage webpage)
        {
            return GetWebpageMetadata().FirstOrDefault(x => x.Type.Name == webpage.WebpageType);
        }

        public int? GetMaxChildNodes(Webpage webpage)
        {
            WebpageMetadata webpageTypeDefinition = GetMetadata(webpage);
            return webpageTypeDefinition.MaxChildNodes > 0
                ? webpageTypeDefinition.MaxChildNodes
                : (int?) null;
        }

        public List<WebpageMetadata> GetValidParentTypes(Webpage webpage)
        {
            Type type = webpage.Unproxy().GetType();
            return GetWebpageMetadata().FindAll(metadata => metadata.ValidChildrenTypes.Contains(type));
        }

        public WebpageMetadata GetMetadataByTypeName(string name)
        {
            return GetWebpageMetadata().FirstOrDefault(x => x.Type.FullName == name);
        }
    }
}