using System;
using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Metadata
{
    public interface IGetWebpageMetadataInfo
    {
        WebpageMetadataInfo Metadata { get; }
    }

    public class WebpageMetadataInfo
    {
        public string Name { get; set; }
        public string IconClass { get; set; }

        public string WebGetController { get; set; }
        public string WebGetAction { get; set; }
        public string WebPostController { get; set; }
        public string WebPostAction { get; set; }
        
        public string WebGetControllerUnauthorized { get; set; }
        public string WebGetActionUnauthorized { get; set; }
        public string WebPostControllerUnauthorized { get;set;  }
        public string WebPostActionUnauthorized { get; set; }

        public string WebGetControllerForbidden { get; set; }
        public string WebGetActionForbidden { get; set; }
        public string WebPostControllerForbidden { get;set;  }
        public string WebPostActionForbidden { get; set; }

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