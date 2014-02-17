using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Indexing
{
    public class WebpageSearchIndexDefinition : IndexDefinition<Webpage>
    {
        private readonly HashSet<IFieldDefinition<WebpageSearchIndexDefinition, Webpage>> _definitions;

        public WebpageSearchIndexDefinition(ISession session,
            IEnumerable<IFieldDefinition<WebpageSearchIndexDefinition, Webpage>> definitions)
            : base(session)
        {
            _definitions = new HashSet<IFieldDefinition<WebpageSearchIndexDefinition, Webpage>>(definitions);
        }

        public override IEnumerable<FieldDefinition<Webpage>> Definitions
        {
            get { return _definitions.Select(definition => definition.GetDefinition); }
        }

        public override IEnumerable<string> FieldNames
        {
            get { return _definitions.Select(definition => definition.Name); }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }

        public override string IndexFolderName
        {
            get { return "UI Webpages"; }
        }

        public override string IndexName
        {
            get { return "UI Webpage Index"; }
        }
    }
}