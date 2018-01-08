using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Attributes;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using Ninject;

namespace MrCMS.Services.Canonical
{
    public class GetPrevAndNextRelTags : IGetPrevAndNextRelTags
    {
        private static readonly Dictionary<Type, Type> GetRelTypes = new Dictionary<Type, Type>();
        private readonly IKernel _kernel;

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

        public GetPrevAndNextRelTags(IKernel kernel)
        {
            _kernel = kernel;
        }

        public string GetPrev(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return null;
            webpage = webpage.Unproxy();
            var baseUrl = webpage.AbsoluteUrl;

            var type = webpage.GetType();
            if (GetRelTypes.ContainsKey(type))
            {
                var getTags = _kernel.Get(GetRelTypes[type]) as GetRelTags;
                if (getTags != null)
                    return getTags.GetPrev(webpage, metadata, viewData);
            }

            if (metadata.IsFirstPage)
                return null;
            if (metadata.PageNumber == 2)
                return baseUrl;
            return $"{baseUrl}?Page={metadata.PageNumber - 1}";
        }

        public string GetNext(Webpage webpage, PagedListMetaData metadata, ViewDataDictionary viewData)
        {
            if (webpage == null)
                return null;
            webpage = webpage.Unproxy();
            var baseUrl = webpage.AbsoluteUrl;

            var type = webpage.GetType();
            if (GetRelTypes.ContainsKey(type))
            {
                var getTags = _kernel.Get(GetRelTypes[type]) as GetRelTags;
                if (getTags != null)
                    return getTags.GetNext(webpage, metadata, viewData);
            }

            if (metadata.IsLastPage)
                return null;
            return $"{baseUrl}?Page={metadata.PageNumber + 1}";
        }
    }
}