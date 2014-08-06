using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class FeatureSectionsDefinition : StringFieldDefinition<FeatureIndexDefinition, Feature>
    {
        public FeatureSectionsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "section", Field.Store.YES, Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Feature obj)
        {
            return FeatureSections(obj).Distinct().Select(section => section.Id.ToString());
        }

        private static IEnumerable<FeatureSection> FeatureSections(Feature obj)
        {
            var webpage = obj.Parent as Webpage;
            foreach (var section in webpage.ActivePages.OfType<FeatureSection>())
            {
                yield return section;
            }
            foreach (var section in obj.OtherSections)
            {
                foreach (var hierarchy in section.ActivePages.OfType<FeatureSection>())
                {
                    yield return hierarchy;
                }
            }
        }
    }
}