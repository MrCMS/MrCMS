using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Attributes;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using X.PagedList;

namespace MrCMS.Services.Canonical
{
    public class GetPrevAndNextRelTags : IGetPrevAndNextRelTags
    {
        private static readonly Dictionary<Type, Type> GetRelTypes = new Dictionary<Type, Type>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IGetLiveUrl _getLiveUrl;

        static GetPrevAndNextRelTags()
        {
            foreach (
                var type in
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                var isSet = false;
                while (typeof(Webpage).IsAssignableFrom(thisType) && !isSet)
                {
                    var formatter = TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(GetRelTags<>).MakeGenericType(thisType)).FirstOrDefault();

                    if (formatter != null)
                    {
                        GetRelTypes[type] = formatter;
                        isSet = true;
                    }

                    thisType = thisType.BaseType;
                }
            }
        }

        public GetPrevAndNextRelTags(IServiceProvider serviceProvider, IGetLiveUrl getLiveUrl)
        {
            _serviceProvider = serviceProvider;
            _getLiveUrl = getLiveUrl;
        }

        public async Task<string> GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return null;
            var baseUrl = await _getLiveUrl.GetAbsoluteUrl(webpage);

            var type = webpage.GetType();
            if (GetRelTypes.ContainsKey(type))
            {
                if (_serviceProvider.GetRequiredService(GetRelTypes[type]) is GetRelTags getTags)
                    return getTags.GetPrev(webpage, metadata, viewData);
            }

            if (metadata.IsFirstPage)
                return null;
            if (metadata.PageNumber == 2)
                return baseUrl;
            return $"{baseUrl}?Page={metadata.PageNumber - 1}";
        }

        public async Task<string> GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return null;
            var baseUrl = await _getLiveUrl.GetAbsoluteUrl(webpage);

            var type = webpage.GetType();
            if (GetRelTypes.ContainsKey(type))
            {
                if (_serviceProvider.GetRequiredService(GetRelTypes[type]) is GetRelTags getTags)
                    return getTags.GetNext(webpage, metadata, viewData);
            }

            if (metadata.IsLastPage)
                return null;
            return $"{baseUrl}?Page={metadata.PageNumber + 1}";
        }
    }
}