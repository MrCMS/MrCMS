using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Entities.Indexes
{
    public class WebpageIndexDefinition : IndexDefinition<Webpage> /*, IRelatedItemIndexDefinition<UrlHistory, Webpage>*/
    {
        private readonly HashSet<IFieldDefinition<WebpageIndexDefinition, Webpage>> _definitions;

        public WebpageIndexDefinition(ISession session, IEnumerable<IFieldDefinition<WebpageIndexDefinition, Webpage>> definitions)
            : base(session)
        {
            _definitions = new HashSet<IFieldDefinition<WebpageIndexDefinition, Webpage>>(definitions);
        }

        public override IEnumerable<FieldDefinition<Webpage>> Definitions
        {
            get { return _definitions.Select(definition => definition.GetDefinition); }
        }

        public override IEnumerable<string> FieldNames
        {
            get { return _definitions.Select(definition => definition.Name); }
        }

        public override string IndexFolderName
        {
            get { return "Webpages"; }
        }

        public override string IndexName
        {
            get { return "Default Webpage Index"; }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }

        public IEnumerable<Webpage> GetEntitiesToUpdate(UrlHistory obj)
        {
            yield return obj.Webpage;
        }
    }
}