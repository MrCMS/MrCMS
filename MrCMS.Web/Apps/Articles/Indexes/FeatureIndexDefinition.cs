using System.Collections.Generic;
using System.Linq;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class FeatureIndexDefinition : IndexDefinition<Feature>
    {
        private readonly IEnumerable<IFieldDefinition<FeatureIndexDefinition, Feature>> _definitions;

        public FeatureIndexDefinition(ISession session,
            IEnumerable<IFieldDefinition<FeatureIndexDefinition, Feature>> definitions)
            : base(session)
        {
            _definitions = definitions;
        }

        public override string IndexFolderName
        {
            get { return "Features"; }
        }

        public override string IndexName
        {
            get { return "Features"; }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }

        public override IEnumerable<FieldDefinition<Feature>> Definitions
        {
            get { return _definitions.Select(definition => definition.GetDefinition); }
        }

        public override IEnumerable<string> FieldNames
        {
            get { return _definitions.Select(definition => definition.Name); }
        }
    }
}