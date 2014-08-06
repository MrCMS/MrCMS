using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class FeaturePrimarySectionDefinition : StringFieldDefinition<FeatureIndexDefinition, Feature>
    {
        public FeaturePrimarySectionDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "primarysection", Field.Store.YES, Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Feature obj)
        {
            yield return obj.Parent.Id.ToString();
        }
    }
}