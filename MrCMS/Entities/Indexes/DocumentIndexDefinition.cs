using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;

namespace MrCMS.Entities.Indexes
{
    public class DocumentIndexDefinition : IndexDefinition<Documents.Document>
    {
        public override string IndexName
        {
            get { return "Default Document Index"; }
        }

        protected override string IndexFolderName
        {
            get { return "Documents"; }
        }

        private static IEnumerable<string> GetAllTypeNames(Documents.Document document)
        {
            if (document is Layout)
                yield return typeof(Layout).FullName;
            else if (document is MediaCategory)
                yield return typeof(MediaCategory).FullName;
            else
            {
                var type = document.Unproxy().GetType();
                while (type != typeof(Webpage))
                {
                    yield return type.FullName;
                    type = type.BaseType;
                }
                yield return typeof(Webpage).FullName;
            }
            yield return typeof(Documents.Document).FullName;
        }

        private static IEnumerable<string> GetParentIds(Documents.Document webpage)
        {
            var parent = webpage.Parent;
            while (parent != null)
            {
                yield return parent.Id.ToString();
                parent = parent.Parent;
            }
        }

        public Analyzer GetAnalyser()
        {
            return _analyser ?? (_analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        public override IEnumerable<FieldDefinition<Documents.Document>> Definitions
        {
            get
            {
                yield return Name;
                yield return Type;
                yield return UrlSegment;
                yield return ParentId;
            }
        }

        public static FieldDefinition<Documents.Document> Name { get { return _name; } }
        public static FieldDefinition<Documents.Document> Type { get { return _type; } }
        public static FieldDefinition<Documents.Document> UrlSegment { get { return _urlSegment; } }
        public static FieldDefinition<Documents.Document> ParentId { get { return _parentId; } }




        private static readonly FieldDefinition<Documents.Document> _name =
            new StringFieldDefinition<Documents.Document>("name", webpage => webpage.Name, Field.Store.YES,
                                         Field.Index.ANALYZED);
        private static readonly FieldDefinition<Documents.Document> _urlSegment =
            new StringFieldDefinition<Documents.Document>("urlsegment", webpage => webpage.UrlSegment, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Documents.Document> _type =
            new StringFieldDefinition<Documents.Document>("type",
             webpage => GetAllTypeNames(webpage), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);


        private static readonly FieldDefinition<Documents.Document> _parentId =
            new StringFieldDefinition<Documents.Document>("parentid", webpage => GetParentIds(webpage), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

    }
}