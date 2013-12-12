using System;
using System.Collections.Generic;
using System.Linq;
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
            new StringFieldDefinition<Document>("urlsegment", webpage => webpage.UrlSegment, Field.Store.YES,
                                                Field.Index.ANALYZED);

        private static readonly FieldDefinition<Document> _type =
            new StringFieldDefinition<Document>("type",
                                                webpage => GetAllTypeNames(webpage), Field.Store.YES,
                                                Field.Index.NOT_ANALYZED);


        private static readonly FieldDefinition<Document> _parentId =
            new StringFieldDefinition<Document>("parentid", webpage => webpage.ParentId.ToString(), Field.Store.NO,
                                                Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _showInNav =
            new StringFieldDefinition<Document>("showinnav",
                                                document =>
                                                (!(document is MediaCategory) ||
                                                 !(document as MediaCategory).HideInAdminNav).ToString(),
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _showChildrenInNav =
            new StringFieldDefinition<Document>("showchildreninnav",
                                                document => ShowChildrenInAdminNav(document).ToString(),
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _iconClass =
            new StringFieldDefinition<Document>("iconclass",
                                                document =>
                                                DocumentMetadataHelper.GetIconClass(document) ?? string.Empty,
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _isRootEntity =
            new StringFieldDefinition<Document>("isrootentity",
                                                document =>
                                                (document.Parent == null).ToString(),
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _displayOrder =
            new IntegerFieldDefinition<Document>("displayorder",
                                                document =>
                                                document.DisplayOrder,
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _isSortable =
            new StringFieldDefinition<Document>("issortable",
                                                document =>
                                                IsDocumentSortable(document).ToString(),
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _maxChildNodes =
            new IntegerFieldDefinition<Document>("maxchildnodes",
                                                document =>
                                                document.GetMaxChildNodes() ?? int.MaxValue,
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _canAddChild =
            new StringFieldDefinition<Document>("canaddchild",
                                                document =>
                                                (!(document is Webpage) ||
                                                 (document as Webpage).GetValidWebpageDocumentTypes().Any()).ToString(),
                                                Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _publishOn =
            new StringFieldDefinition<Document>("publishOn",
                                                document => document is Webpage
                                                                ? DateTools.DateToString((document as Webpage).PublishOn.GetValueOrDefault(DateTime.MaxValue), DateTools.Resolution.SECOND)
                                                                : string.Empty, Field.Store.YES,
                                                Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Document> _revealInNavigation =
            new StringFieldDefinition<Document>("revealInNavigation",
                                                document => ((document is Webpage) &&
                                                                     (document as Webpage).RevealInNavigation).ToString(),
                                                                     Field.Store.YES, Field.Index.NOT_ANALYZED);


        public override string IndexName
        {
            get { return "Default Document Index"; }
        }

        public override string IndexFolderName
        {
            get { return "Documents"; }
        }

        private static bool ShowChildrenInAdminNav(Document document)
        {
            var documentTypeDefinition = DocumentMetadataHelper.GetMetadataByType(document.GetType());
            return documentTypeDefinition == null || documentTypeDefinition.ShowChildrenInAdminNav;
        }

        public override IEnumerable<FieldDefinition<Document>> Definitions
        {
            get
            {
                yield return Name;
                yield return Type;
                yield return UrlSegment;
                yield return ParentId;
                yield return ShowInNav;
                yield return ShowChildrenInNav;
                yield return IconClass;
                yield return IsRootEntity;
                yield return DisplayOrder;
                yield return IsSortable;
                yield return MaxChildNodes;
                yield return CanAddChild;
                yield return PublishOn;
                yield return RevealInNavigation;
            }
        }

        private static bool IsDocumentSortable(Document document)
        {
            var documentTypeDefinition = DocumentMetadataHelper.GetMetadataByType(document.GetType());
            return documentTypeDefinition != null && documentTypeDefinition.Sortable;
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

        public static FieldDefinition<Document> ShowInNav
        {
            get { return _showInNav; }
        }

        public static FieldDefinition<Document> ShowChildrenInNav
        {
            get { return _showChildrenInNav; }
        }

        public static FieldDefinition<Document> IconClass
        {
            get { return _iconClass; }
        }

        public static FieldDefinition<Document> IsRootEntity
        {
            get { return _isRootEntity; }
        }

        public static FieldDefinition<Document> DisplayOrder
        {
            get { return _displayOrder; }
        }

        public static FieldDefinition<Document> IsSortable
        {
            get { return _isSortable; }
        }

        public static FieldDefinition<Document> MaxChildNodes
        {
            get { return _maxChildNodes; }
        }

        public static FieldDefinition<Document> CanAddChild
        {
            get { return _canAddChild; }
        }
        public static FieldDefinition<Document> PublishOn { get { return _publishOn; } }
        public static FieldDefinition<Document> RevealInNavigation { get { return _revealInNavigation; } }

        private static IEnumerable<string> GetAllTypeNames(Document document)
        {
            if (document is Layout)
                yield return typeof(Layout).FullName;
            else if (document is MediaCategory)
                yield return typeof(MediaCategory).FullName;
            else
            {
                Type type = document.Unproxy().GetType();
                while (type != typeof(Webpage))
                {
                    yield return type.FullName;
                    type = type.BaseType;
                }
                yield return typeof(Webpage).FullName;
            }
            yield return typeof(Document).FullName;
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