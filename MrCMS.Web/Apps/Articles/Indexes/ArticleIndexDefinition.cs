using System.Collections.Generic;
using System.Linq;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class ArticleIndexDefinition : IndexDefinition<Article>
    {
        private readonly IEnumerable<IFieldDefinition<ArticleIndexDefinition, Article>> _definitions;

        public ArticleIndexDefinition(ISession session,
            IEnumerable<IFieldDefinition<ArticleIndexDefinition, Article>> definitions)
            : base(session)
        {
            _definitions = definitions;
        }

        public override string IndexFolderName
        {
            get { return "Articles"; }
        }

        public override string IndexName
        {
            get { return "Articles"; }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }

        public override IEnumerable<FieldDefinition<Article>> Definitions
        {
            get { return _definitions.Select(definition => definition.GetDefinition); }
        }

        public override IEnumerable<string> FieldNames
        {
            get { return _definitions.Select(definition => definition.Name); }
        }
    }
}