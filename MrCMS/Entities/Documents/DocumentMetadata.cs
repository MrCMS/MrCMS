using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using System.Linq;
using NHibernate;

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

        public SortBy SortBy { get; set; }

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

        public bool ShowChildrenInAdminNav { get; set; }

        public bool ChildrenMaintainHierarchy { get; set; }

        public bool HasBodyContent { get; set; }

        public string App { get; set; }

        public IEnumerable<Type> ValidChildrenTypes
        {
            get
            {
                switch (ChildrenListType)
                {
                    case ChildrenListType.BlackList:
                        return
                            DocumentMetadataHelper.DocumentMetadatas.Where(
                                metadata => !ChildrenList.Contains(metadata.Type) && !metadata.AutoBlacklist)
                                                  .Select(metadata => metadata.Type)
                                                  .ToList();
                    case ChildrenListType.WhiteList:
                        return ChildrenList;
                }
                return new List<Type>();
            }
        }

        public bool RevealInNavigation { get; set; }
    }

    public enum SortBy 
    {
        DisplayOrder,
        DisplayOrderDesc,
        PublishedOn,
        PublishedOnDesc,
        CreatedOn,
        CreatedOnDesc,
    }

    public enum ChildrenListType
    {
        BlackList,
        WhiteList
    }
}
