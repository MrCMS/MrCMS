using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapElementAppender : ISitemapElementAppender
    {
        private static readonly Dictionary<string, Type> SitemapGenerationInfoTypes;
        private readonly IKernel _kernel;
        private readonly IEnumerable<IReasonToExcludePageFromSitemap> _exclusionReasons;

        static SitemapElementAppender()
        {
            SitemapGenerationInfoTypes = new Dictionary<string, Type>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                bool processed = false;
                while (!processed && thisType != null && thisType != typeof(Webpage))
                {
                    var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(SitemapGenerationInfo<>).MakeGenericType(thisType));
                    if (types.Any())
                    {
                        SitemapGenerationInfoTypes.Add(type.FullName, types.First());
                        processed = true;
                    }
                    thisType = thisType.BaseType;
                }
            }
        }

        public SitemapElementAppender(IKernel kernel, IEnumerable<IReasonToExcludePageFromSitemap> exclusionReasons)
        {
            _kernel = kernel;
            _exclusionReasons = exclusionReasons;
        }

        private ISitemapGenerationInfo GetInfo(Webpage webpage)
        {
            ISitemapGenerationInfo generationInfo = null;
            if (webpage == null)
                return null;

            var typeName = webpage.GetType().FullName;
            if (SitemapGenerationInfoTypes.ContainsKey(typeName))
            {
                generationInfo = _kernel.Get(SitemapGenerationInfoTypes[typeName]) as ISitemapGenerationInfo;
            }
            return generationInfo ?? _kernel.Get<DefaultSitemapGenerationInfo>();
        }

        public void AddCustomSiteMapData(Webpage webpage, XElement urlset, XDocument xmlDocument)
        {
            if (webpage == null)
                return;
            var info = GetInfo(webpage);
            info.Append(webpage, urlset, xmlDocument);
        }

        public bool ShouldAppend(Webpage webpage)
        {
            if (webpage == null)
                return false;
            if (_exclusionReasons.Any(sitemap => sitemap.ShouldExclude(webpage)))
                return false;
            var info = GetInfo(webpage);
            return info.ShouldAppend(webpage);
        }
    }
}