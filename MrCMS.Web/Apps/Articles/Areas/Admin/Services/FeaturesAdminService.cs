using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Indexes;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class FeaturesAdminService : IFeaturesAdminService
    {
        private readonly IGetFeatureSectionOptions _getFeatureSectionOptions;
        private readonly ISearcher<Feature, FeatureIndexDefinition> _featureSearcher;

        public FeaturesAdminService(ISearcher<Feature, FeatureIndexDefinition> featureSearcher, IGetFeatureSectionOptions getFeatureSectionOptions)
        {
            _featureSearcher = featureSearcher;
            _getFeatureSectionOptions = getFeatureSectionOptions;
        }

        public IPagedList<Feature> Search(FeaturesSearchQuery query)
        {
            var booleanQuery = new BooleanQuery();
            if (query.SectionId.HasValue)
            {
                booleanQuery.Add(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<FeatureSectionsDefinition>(),
                        query.SectionId.ToString())), Occur.MUST);
            }

            var sort = new Sort(new[]
            {
                new SortField(FieldDefinition.GetFieldName<FeaturePublishedDefinition>(),
                    SortField.STRING, true)
            });
            Query luceneQuery = booleanQuery.Clauses.Any() ? (Query) booleanQuery : new MatchAllDocsQuery();
            return _featureSearcher.Search(luceneQuery, query.Page,sort:sort);
        }

        public List<SelectListItem> GetFeatureSectionOptions()
        {
            return _getFeatureSectionOptions.GetOptions();
        }

        public List<SelectListItem> GetPrimarySectionOptions()
        {
            var featureSectionOptions = GetFeatureSectionOptions();
            featureSectionOptions.Insert(0, SelectListItemHelper.EmptyItem("Any"));
            return featureSectionOptions;
        }
    }
}