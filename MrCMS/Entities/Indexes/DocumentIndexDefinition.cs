using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Utils;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Entities.Indexes
{
    public class DocumentIndexDefinition : IIndexDefinition<Documents.Document>
    {
        private Analyzer _analyser;
        private static readonly FieldDefinition<Documents.Document> _id =
            new FieldDefinition<Documents.Document>("id", webpage => webpage.Id.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Documents.Document> _name =
            new FieldDefinition<Documents.Document>("name", webpage => webpage.Name, Field.Store.YES,
                                         Field.Index.ANALYZED);
        private static readonly FieldDefinition<Documents.Document> _urlSegment =
            new FieldDefinition<Documents.Document>("urlsegment", webpage => webpage.UrlSegment, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Documents.Document> _type =
            new FieldDefinition<Documents.Document>("type",
             webpage => GetAllTypeNames(webpage), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);


        private static readonly FieldDefinition<Documents.Document> _parentId =
            new FieldDefinition<Documents.Document>("parentid", webpage => GetParentIds(webpage), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

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

        public Documents.Document Convert(ISession session, Document document)
        {
            return session.Get<Documents.Document>(document.GetValue<int>("id"));
        }

        public IEnumerable<Documents.Document> Convert(ISession session, IEnumerable<Document> documents)
        {
            return documents.Select(document => Convert(session, document));
        }

        public string GetLocation(CurrentSite currentSite)
        {
            string location = string.Format("~/App_Data/Indexes/{0}/Documents/", currentSite.Id);
            string mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return _analyser ?? (_analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        public IEnumerable<FieldDefinition<Documents.Document>> Definitions
        {
            get
            {
                yield return Id;
                yield return Name;
                yield return Type;
                yield return UrlSegment;
                yield return ParentId;
            }
        }

        public static FieldDefinition<Documents.Document> Id { get { return _id; } }
        public static FieldDefinition<Documents.Document> Name { get { return _name; } }
        public static FieldDefinition<Documents.Document> Type { get { return _type; } }
        public static FieldDefinition<Documents.Document> UrlSegment { get { return _urlSegment; } }
        public static FieldDefinition<Documents.Document> ParentId { get { return _parentId; } }



        public string IndexName
        {
            get { return "Default Document Index"; }
        }

        public Document Convert(Documents.Document entity)
        {
            return new Document().SetFields(Definitions, entity);
        }

        public Term GetIndex(Documents.Document entity)
        {
            return new Term("id", entity.Id.ToString());
        }
    }
}