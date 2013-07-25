using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using Document = MrCMS.Entities.Documents.Document;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Entities.Indexes
{
    public class DocumentIndexDefinition : IndexDefinition<Document>
    {
        private static readonly FieldDefinition<Document> _name =
            new StringFieldDefinition<Document>("name", webpage => webpage.Name, Field.Store.YES,
                                                Field.Index.ANALYZED);

        private static readonly FieldDefinition<Document> _urlSegment =
            new StringFieldDefinition<Document>("urlsegment", webpage => webpage.UrlSegment, Field.Store.NO,
                                                Field.Index.ANALYZED);

        private static readonly FieldDefinition<Document> _type =
            new StringFieldDefinition<Document>("type",
                                                webpage => GetAllTypeNames(webpage), Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);


        private static readonly FieldDefinition<Document> _parentId =
            new StringFieldDefinition<Document>("parentid", webpage => GetParentIds(webpage), Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);

        public override string IndexName
        {
            get { return "Default Document Index"; }
        }

        protected override string IndexFolderName
        {
            get { return "Documents"; }
        }

        public override IEnumerable<FieldDefinition<Document>> Definitions
        {
            get
            {
                yield return Name;
                yield return Type;
                yield return UrlSegment;
                yield return ParentId;
            }
        }

        public static FieldDefinition<Document> Name
        {
            get { return _name; }
        }

        public static FieldDefinition<Document> Type
        {
            get { return _type; }
        }

        public static FieldDefinition<Document> UrlSegment
        {
            get { return _urlSegment; }
        }

        public static FieldDefinition<Document> ParentId
        {
            get { return _parentId; }
        }

        private static IEnumerable<string> GetAllTypeNames(Document document)
        {
            if (document is Layout)
                yield return typeof (Layout).FullName;
            else if (document is MediaCategory)
                yield return typeof (MediaCategory).FullName;
            else
            {
                Type type = document.Unproxy().GetType();
                while (type != typeof (Webpage))
                {
                    yield return type.FullName;
                    type = type.BaseType;
                }
                yield return typeof (Webpage).FullName;
            }
            yield return typeof (Document).FullName;
        }

        private static IEnumerable<string> GetParentIds(Document webpage)
        {
            Document parent = webpage.Parent;
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
    }
}