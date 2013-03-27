using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Utils;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Entities.Indexes
{
    public class WebpageIndexDefinition : IIndexDefinition<Webpage>
    {
        private Analyzer _analyser;

        public Document Convert(Webpage entity)
        {
            return new Document().SetFields(Definitions, entity);
        }

        public IEnumerable<FieldDefinition<Webpage>> Definitions
        {
            get
            {
                yield return Id;
                yield return Name;
                yield return BodyContent;
                yield return MetaTitle;
                yield return MetaKeywords;
                yield return MetaDescription;
                yield return UrlSegment;
                yield return Type;
                yield return PublishOn;
            }
        }

        public static FieldDefinition<Webpage> Id { get { return _id; } }
        public static FieldDefinition<Webpage> Name { get { return _name; } }
        public static FieldDefinition<Webpage> BodyContent { get { return _bodyContent; } }
        public static FieldDefinition<Webpage> MetaTitle { get { return _metaTitle; } }
        public static FieldDefinition<Webpage> MetaKeywords { get { return _metaKeywords; } }
        public static FieldDefinition<Webpage> MetaDescription { get { return _metaDescription; } }
        public static FieldDefinition<Webpage> UrlSegment { get { return _urlSegment; } }
        public static FieldDefinition<Webpage> Type { get { return _type; } }
        public static FieldDefinition<Webpage> PublishOn { get { return _publishOn; } }

        public Term GetIndex(Webpage entity)
        {
            return new Term("id", entity.Id.ToString());
        }

        public Webpage Convert(ISession session, Document document)
        {
            return session.Get<Webpage>(document.GetValue<int>("id"));
        }

        public IEnumerable<Webpage> Convert(ISession session, IEnumerable<Document> documents)
        {
            List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            return
                ids.Chunk(100)
                   .SelectMany(
                       ints => session.QueryOver<Webpage>().Where(webpage => webpage.Id.IsIn(ints.ToList())).List());
        }

        public string GetLocation(CurrentSite currentSite)
        {
            string location = string.Format("~/App_Data/Indexes/{0}/Webpages/", currentSite.Id);
            string mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return _analyser = _analyser ?? new StandardAnalyzer(Version.LUCENE_30);
        }

        public string IndexName
        {
            get { return "Default Webpage Index"; }
        }

        private static readonly FieldDefinition<Webpage> _id =
            new FieldDefinition<Webpage>("id", webpage => webpage.Id.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Webpage> _name =
            new FieldDefinition<Webpage>("name", webpage => webpage.Name, Field.Store.YES,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _bodyContent =
            new FieldDefinition<Webpage>("bodycontent", webpage => webpage.BodyContent, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _metaTitle =
            new FieldDefinition<Webpage>("metatitle", webpage => webpage.MetaTitle, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _metaKeywords =
            new FieldDefinition<Webpage>("metakeywords", webpage => webpage.MetaKeywords, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _metaDescription =
            new FieldDefinition<Webpage>("metadescription",
                                         webpage => webpage.MetaDescription, Field.Store.NO, Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _urlSegment =
            new FieldDefinition<Webpage>("urlsegment", webpage => webpage.UrlSegment, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _type =
            new FieldDefinition<Webpage>("type", webpage => webpage.GetType().FullName, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _publishOn =
            new FieldDefinition<Webpage>("publishOn",
                                         webpage =>
                                         DateTools.DateToString(webpage.PublishOn.GetValueOrDefault(DateTime.MaxValue),
                                                                DateTools.Resolution.SECOND), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);
    }
}