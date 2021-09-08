using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MrCMS.Services;

namespace MrCMS.Entities.Documents
{
    public readonly struct DocumentMetadata
    {
        public DocumentMetadata(string name, string iconClass,
            string webGetController, string webGetAction, string webPostController, string webPostAction,
            string webGetControllerUnauthorized, string webGetActionUnauthorized, string webPostControllerUnauthorized,
            string webPostActionUnauthorized,
            string webGetControllerForbidden, string webGetActionForbidden, string webPostControllerForbidden,
            string webPostActionForbidden,
            int maxChildNodes, bool sortable, SortBy sortBy,
            int displayOrder, Type type, Type[] postTypes, ChildrenListType childrenListType,
            IEnumerable<Type> childrenList, bool autoBlacklist, bool requiresParent, string defaultLayoutName,
            string editPartialView, bool showChildrenInAdminNav, bool childrenMaintainHierarchy, bool hasBodyContent,
            bool revealInNavigation, IEnumerable<Type> validChildrenTypes)
        {
            Name = name;
            IconClass = iconClass;
            WebGetController = webGetController;
            WebGetAction = webGetAction;
            WebPostController = webPostController;
            WebPostAction = webPostAction;
            WebGetControllerUnauthorized = webGetControllerUnauthorized;
            WebGetActionUnauthorized = webGetActionUnauthorized;
            WebPostControllerUnauthorized = webPostControllerUnauthorized;
            WebPostActionUnauthorized = webPostActionUnauthorized;
            WebGetControllerForbidden = webGetControllerForbidden;
            WebGetActionForbidden = webGetActionForbidden;
            WebPostControllerForbidden = webPostControllerForbidden;
            WebPostActionForbidden = webPostActionForbidden;
            MaxChildNodes = maxChildNodes;
            Sortable = sortable;
            SortBy = sortBy;
            DisplayOrder = displayOrder;
            Type = type;
            PostTypes = postTypes;
            ChildrenListType = childrenListType;
            ChildrenList = childrenList;
            AutoBlacklist = autoBlacklist;
            RequiresParent = requiresParent;
            DefaultLayoutName = defaultLayoutName;
            EditPartialView = editPartialView;
            ShowChildrenInAdminNav = showChildrenInAdminNav;
            ChildrenMaintainHierarchy = childrenMaintainHierarchy;
            HasBodyContent = hasBodyContent;
            ValidChildrenTypes = validChildrenTypes;
            RevealInNavigation = revealInNavigation;
        }

        public string Name { get; }
        public string IconClass { get; }

        public string WebGetController { get; }
        public string WebGetAction { get; }
        public string WebPostController { get; }
        public string WebPostAction { get; }

        public string WebGetControllerUnauthorized { get; }
        public string WebGetActionUnauthorized { get; }
        public string WebPostControllerUnauthorized { get; }
        public string WebPostActionUnauthorized { get; }

        public string WebGetControllerForbidden { get; }
        public string WebGetActionForbidden { get; }
        public string WebPostControllerForbidden { get; }
        public string WebPostActionForbidden { get; }

        public int MaxChildNodes { get; }

        public bool Sortable { get; }

        public SortBy SortBy { get; }

        public int DisplayOrder { get; }
        public Type Type { get; }
        public Type[] PostTypes { get; }

        public string TypeName => Type == null ? string.Empty : Type.Name;

        public ChildrenListType ChildrenListType { get; }
        public IEnumerable<Type> ChildrenList { get; }
        public bool AutoBlacklist { get; }
        public bool RequiresParent { get; }

        public string DefaultLayoutName { get; }

        public string EditPartialView { get; }

        public bool ShowChildrenInAdminNav { get; }

        public bool ChildrenMaintainHierarchy { get; }

        public bool HasBodyContent { get; }

        public IEnumerable<Type> ValidChildrenTypes
        {
            get;
            // get
            // {
            //     switch (ChildrenListType)
            //     {
            //         case ChildrenListType.BlackList:
            //             return 
            //                 DocumentMetadataHelper.DocumentMetadatas.Where(
            //                     metadata => !ChildrenList.Contains(metadata.Type) && !metadata.AutoBlacklist)
            //                                       .Select(metadata => metadata.Type)
            //                                       .ToList();
            //         case ChildrenListType.WhiteList:
            //             return ChildrenList;
            //     }
            //     return new List<Type>();
            // }
        }

        public bool RevealInNavigation { get; }

        public string GetController(string method, PageAccessPermission permission)
        {
            if (HttpMethods.IsPost(method))
                return GetPostController(permission); //WebPostController;
            if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method))
                return GetGetController(permission); // WebGetController;
            return null;
        }

        private string GetGetController(PageAccessPermission permission)
        {
            return permission switch
            {
                PageAccessPermission.Allowed => WebGetController,
                PageAccessPermission.Unauthorized => WebGetControllerUnauthorized,
                PageAccessPermission.Forbidden => WebGetControllerForbidden,
                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
            };
        }

        private string GetPostController(PageAccessPermission permission)
        {
            return permission switch
            {
                PageAccessPermission.Allowed => WebPostController,
                PageAccessPermission.Unauthorized => WebPostControllerUnauthorized,
                PageAccessPermission.Forbidden => WebPostControllerForbidden,
                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
            };
        }

        public string GetAction(string method, PageAccessPermission permission)
        {
            if (HttpMethods.IsPost(method))
                return GetPostAction(permission); // WebPostAction;
            if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method))
                return GetGetAction(permission); //WebGetAction;
            return null;
        }

        private string GetGetAction(PageAccessPermission permission)
        {
            return permission switch
            {
                PageAccessPermission.Allowed => WebGetAction,
                PageAccessPermission.Unauthorized => WebGetActionUnauthorized,
                PageAccessPermission.Forbidden => WebGetActionForbidden,
                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
            };
        }

        private string GetPostAction(PageAccessPermission permission)
        {
            return permission switch
            {
                PageAccessPermission.Allowed => WebPostAction,
                PageAccessPermission.Unauthorized => WebPostActionUnauthorized,
                PageAccessPermission.Forbidden => WebPostActionForbidden,
                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
            };
        }
    }
}