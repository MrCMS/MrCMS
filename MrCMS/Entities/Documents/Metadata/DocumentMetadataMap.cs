using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Entities.Documents.Metadata
{
    public abstract class DocumentMetadataMap<T> : IGetDocumentMetadata where T : Document
    {
        public virtual string Name { get { return typeof(T).Name.BreakUpString(); } }
        public virtual string IconClass { get { return "icon-file"; } }

        public virtual string WebGetController { get { return typeof(T).Name; } }
        public virtual string WebGetAction { get { return "Show"; } }
        public virtual string WebPostController { get { return typeof(T).Name; } }
        public virtual string WebPostAction { get { return "Post"; } }

        public virtual Func<Document, object> SortBy
        {
            get { return document => document.DisplayOrder; }
        }

        public virtual string App { get { return null; } }

        public virtual int MaxChildNodes { get { return 15; } }

        public virtual bool SortByDesc { get { return false; } }
        public virtual bool Sortable { get { return true; } }

        public virtual int DisplayOrder { get { return 1; } }
        public Type Type { get { return typeof(T); } }
        public virtual Type[] PostTypes { get { return new Type[0]; } }

        public virtual ChildrenListType ChildrenListType { get { return ChildrenListType.BlackList; } }
        public virtual IEnumerable<Type> ChildrenList { get { return Enumerable.Empty<Type>(); } }
        public virtual bool AutoBlacklist { get { return false; } }
        public virtual bool RequiresParent { get { return false; } }

        public virtual string DefaultLayoutName { get { return "Three Column"; } }

        public DocumentMetadata Metadata
        {
            get
            {
                return new DocumentMetadata
                           {
                               Name = Name,
                               IconClass = IconClass,
                               WebGetAction = WebGetAction,
                               WebGetController = WebGetController,
                               WebPostAction = WebPostAction,
                               WebPostController = WebPostController,
                               SortBy = SortBy,
                               MaxChildNodes = MaxChildNodes,
                               SortByDesc = SortByDesc,
                               Sortable = Sortable,
                               DisplayOrder = DisplayOrder,
                               Type = Type,
                               PostTypes = PostTypes,
                               ChildrenListType = ChildrenListType,
                               ChildrenList = ChildrenList,
                               AutoBlacklist = AutoBlacklist,
                               RequiresParent = RequiresParent,
                               DefaultLayoutName = DefaultLayoutName
                           };
            }
        }
    }
}