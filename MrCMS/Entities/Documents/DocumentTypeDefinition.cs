using System;

namespace MrCMS.Entities.Documents
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentTypeDefinition : Attribute
    {
        public DocumentTypeDefinition(ChildrenListType childrenListType, params Type[] childrenList)
        {
            ChildrenListType = childrenListType;
            ChildrenList = childrenList;
        }

        public string Name { get; set; }
        public string IconClass { get; set; }

        /// <summary>
        /// The controller used to render this type of view, by convention will use the abstract document type (Webpage)
        /// </summary>
        public string WebGetController { get; set; }
        /// <summary>
        /// The action used to render this type of view, by convention will use the document type
        /// </summary>
        public string WebGetAction { get; set; }
        /// <summary>
        /// The controller used to render this type of view, by convention will use the abstract document type (Webpage)
        /// </summary>
        public string WebPostController { get; set; }
        /// <summary>
        /// The action used to render this type of view, by convention will use the document type
        /// </summary>
        public string WebPostAction { get; set; }
        private string _sortBy = DisplayOrderText;
        private const string DisplayOrderText = "DisplayOrder";

        public string SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; }
        }

        public int MaxChildNodes { get; set; }

        public bool CustomSort
        {
            get { return SortBy != DisplayOrderText; }
        }

        public bool SortByDesc { get; set; }

        public int DisplayOrder { get; set; }
        public Type Type { get; set; }
        public string TypeName { get { return Type == null ? string.Empty : Type.Name; } }

        public ChildrenListType ChildrenListType { get; set; }
        public Type[] ChildrenList { get; set; }
        public bool AutoBlacklist { get; set; }
        public bool RequiresParent { get; set; }

        public string DefaultLayoutName { get; set; }
    }

    public enum ChildrenListType
    {
        BlackList,
        WhiteList
    }
}
