using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Entities.Indexes
{
    public class AdminWebpageIndexDefinition : IndexDefinition<Webpage>
    {
        private readonly HashSet<IFieldDefinition<AdminWebpageIndexDefinition, Webpage>> _definitions;

        public AdminWebpageIndexDefinition(ISession session, IEnumerable<IFieldDefinition<AdminWebpageIndexDefinition, Webpage>> definitions)
            : base(session)
        {
            _definitions = new HashSet<IFieldDefinition<AdminWebpageIndexDefinition, Webpage>>(definitions);
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
            get { return "WebpagesAdmin"; }
        }

        public override string IndexName
        {
            get { return "Default Admin Webpage Index"; }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }
    }
}