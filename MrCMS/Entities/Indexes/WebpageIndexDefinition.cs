using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;

namespace MrCMS.Entities.Indexes
{
    public class WebpageIndexDefinition : IndexDefinition<Webpage>
    {

        public override IEnumerable<FieldDefinition<Webpage>> Definitions
        {
            get
            {
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

        public static FieldDefinition<Webpage> Name { get { return _name; } }
        public static FieldDefinition<Webpage> BodyContent { get { return _bodyContent; } }
        public static FieldDefinition<Webpage> MetaTitle { get { return _metaTitle; } }
        public static FieldDefinition<Webpage> MetaKeywords { get { return _metaKeywords; } }
        public static FieldDefinition<Webpage> MetaDescription { get { return _metaDescription; } }
        public static FieldDefinition<Webpage> UrlSegment { get { return _urlSegment; } }
        public static FieldDefinition<Webpage> Type { get { return _type; } }
        public static FieldDefinition<Webpage> PublishOn { get { return _publishOn; } }

        public override string IndexFolderName
        {
            get { return "Webpages"; }
        }

        public override string IndexName
        {
            get { return "Default Webpage Index"; }
        }

        private static readonly FieldDefinition<Webpage> _name =
            new StringFieldDefinition<Webpage>("name", webpage => webpage.Name, Field.Store.YES,
                                               Field.Index.ANALYZED, 2f);

        private static readonly FieldDefinition<Webpage> _bodyContent =
            new StringFieldDefinition<Webpage>("bodycontent", webpage => webpage.BodyContent, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _metaTitle =
            new StringFieldDefinition<Webpage>("metatitle", webpage => webpage.MetaTitle, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _metaKeywords =
            new StringFieldDefinition<Webpage>("metakeywords", webpage => webpage.MetaKeywords, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _metaDescription =
            new StringFieldDefinition<Webpage>("metadescription",
                                         webpage => webpage.MetaDescription, Field.Store.NO, Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _urlSegment =
            new StringFieldDefinition<Webpage>("urlsegment", webpage => webpage.UrlSegment, Field.Store.NO,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<Webpage> _type =
            new StringFieldDefinition<Webpage>("type",
             webpage => GetAllTypeNames(webpage), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Webpage> _publishOn =
            new StringFieldDefinition<Webpage>("publishOn",
                                         webpage =>
                                         DateTools.DateToString(webpage.PublishOn.GetValueOrDefault(DateTime.MaxValue),
                                                                DateTools.Resolution.SECOND), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

        private static IEnumerable<string> GetAllTypeNames(Documents.Document document)
        {
            var type = document.Unproxy().GetType();
            while (type != typeof(Webpage))
            {
                yield return type.FullName;
                type = type.BaseType;
            }

            yield return typeof(Webpage).FullName;
        }
    }
}