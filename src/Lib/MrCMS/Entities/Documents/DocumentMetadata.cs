using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Http;
using NHibernate;

namespace MrCMS.Entities.Documents
{
    public readonly struct DocumentMetadata
    {
        public DocumentMetadata(string name, string iconClass, string webGetController, string webGetAction,
            string webPostController, string webPostAction, int maxChildNodes, bool sortable, SortBy sortBy,
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

        public int MaxChildNodes { get; }

        public bool Sortable { get; }

        public SortBy SortBy { get; }

        public int DisplayOrder { get; }
        public Type Type { get; }
        public Type[] PostTypes { get; }

        public string TypeName
        {
            get { return Type == null ? string.Empty : Type.Name; }
        }

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

        public string GetController(string method)
        {
            if (HttpMethods.IsPost(method))
                return WebPostController;
            if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method))
                return WebGetController;
            return null;
        }

        public string GetAction(string method)
        {
            if (HttpMethods.IsPost(method))
                return WebPostAction;
            if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method))
                return WebGetAction;
            return null;
        }
    }
}