using System;
using System.Collections.Generic;

namespace MrCMS.Entities.Documents
{
    public class DocumentMetadata
    {
        public string Name { get; set; }
        public string IconClass { get; set; }

        public string WebGetController { get; set; }
        public string WebGetAction { get; set; }
        public string WebPostController { get; set; }
        public string WebPostAction { get; set; }

        public int MaxChildNodes { get; set; }

        public bool Sortable { get; set; }
	
	    public Func<Document, object> SortBy { get; set; }

        public bool SortByDesc { get; set; }

        public int DisplayOrder { get; set; }
        public Type Type { get; set; }
        public Type[] PostTypes { get; set; }
        public string TypeName { get { return Type == null ? string.Empty : Type.Name; } }

        public ChildrenListType ChildrenListType { get; set; }
        public IEnumerable<Type> ChildrenList { get; set; }
        public bool AutoBlacklist { get; set; }
        public bool RequiresParent { get; set; }

        public string DefaultLayoutName { get; set; }

        public string EditPartialView { get; set; }
    }

    public enum ChildrenListType
    {
        BlackList,
        WhiteList
    }
}
