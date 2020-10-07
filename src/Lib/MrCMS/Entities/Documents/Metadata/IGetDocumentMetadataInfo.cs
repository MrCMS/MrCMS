using System;
using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Metadata
{
    public interface IGetDocumentMetadataInfo
    {
        DocumentMetadataInfo Metadata { get; }
    }

    public class DocumentMetadataInfo
    {
        public string Name { get; set; }
        public string IconClass { get; set; }

        public string WebGetController { get; set; }
        public string WebGetAction { get; set; }
        public string WebPostController { get; set; }
        public string WebPostAction { get; set; }

        public int MaxChildNodes { get; set; }

        public bool Sortable { get; set; }

        public SortBy SortBy { get; set; }

        public int DisplayOrder { get; set; }
        public Type Type { get; set; }
        public Type[] PostTypes { get; set; }

        public ChildrenListType ChildrenListType { get; set; }
        public IEnumerable<Type> ChildrenList { get; set; }
        public bool AutoBlacklist { get; set; }
        public bool RequiresParent { get; set; }

        public string DefaultLayoutName { get; set; }

        public string EditPartialView { get; set; }

        public bool ShowChildrenInAdminNav { get; set; }

        public bool ChildrenMaintainHierarchy { get; set; }

        public bool HasBodyContent { get; set; }
        public bool RevealInNavigation { get; set; }
    }
}