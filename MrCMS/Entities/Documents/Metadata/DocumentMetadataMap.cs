using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Entities.Documents.Metadata
{
    /// <summary>
    /// Abstract class useed to define metadata of your documents within Mr CMS
    /// </summary>
    /// <typeparam name="T">DocumentType</typeparam>
    public abstract class DocumentMetadataMap<T> : IGetDocumentMetadata where T : Webpage, new()
    {
        /// <summary>
        /// Name of the document type for display when adding a page in Mr CMS. I.E Text Page, Blog Page etc
        /// </summary>
        public virtual string Name { get { return typeof(T).Name.BreakUpString(); } }

        /// <summary>
        /// The icon shown next to the page type in Mr CMS admin. By default icons are taken from bootstrap
        /// </summary>
        public virtual string IconClass { get { return "icon-file"; } }

        /// <summary>
        /// Controller used to render this pagetype when a GET/HEAD request is made. By Default is 'Webpage'
        /// </summary>
        public virtual string WebGetController { get { return "Webpage"; } }
        /// <summary>
        /// Action method on specified controller used to render this pagetype when a GET request is made. By Default is 'Show'
        /// </summary>
        public virtual string WebGetAction { get { return "Show"; } }

        /// <summary>
        /// Controller used to render this pagetype when a POST request is made. By convention is the name of the Document Controller.
        /// </summary>
        public virtual string WebPostController { get { return typeof(T).Name; } }
        /// <summary>
        /// Action method on specified controller used to render this pagetype when a POST request is made. By Default is 'Post'
        /// </summary>
        public virtual string WebPostAction { get { return "Post"; } }
        /// <summary>
        /// For advanced use where complex processes are required. Take an eCommerce payment system for example.
        /// You need PaymentInfo and other models to be passed to the action methods on contollers. 
        /// Use Post Types to define these objects. Alternatively use FormCollection if strong typing is not required.
        /// </summary>
        public virtual Type[] PostTypes { get { return new Type[0]; } }

        /// <summary>
        /// Used to overide display order by another document property. I.E display by document date.
        /// </summary>
        public virtual SortBy SortBy
        {
            get { return SortBy.DisplayOrder; }
        }

        /// <summary>
        /// Determines whether children of this document type are sortable
        /// </summary>
        public virtual bool Sortable { get { return true; } }

        /// <summary>
        /// Number of child notdse to show in web tree within admin. Usefull if there are to be many document nested I.E News / Blog etc
        /// </summary>
        public virtual int MaxChildNodes { get { return 15; } }

        /// <summary>
        /// Display order in admin when adding a page
        /// </summary>
        public virtual int DisplayOrder { get { return 1; } }

        /// <summary>
        /// System type
        /// </summary>
        public Type Type { get { return typeof(T); } }

        /// <summary>
        /// Determines the allowed children for the Document. 
        /// </summary>
        public virtual ChildrenListType ChildrenListType { get { return ChildrenListType.BlackList; } }

        /// <summary>
        /// If ChildrenListType is BlackList, any Types passed in here will not be allowed to be added to this Document Type. And vice-versa, if ChildrenListType is Whitelist, Types passed in ChildrenList will be allowed only.
        /// </summary>
        public virtual IEnumerable<Type> ChildrenList { get { return Enumerable.Empty<Type>(); } }

        /// <summary>
        /// If set to true, type must be added to white list to be a child.
        /// </summary>
        public virtual bool AutoBlacklist { get { return false; } }

        /// <summary>
        /// Specifies if the document can be a route page. For example a blog page would need to be underneath a blog container and this value would need to be true.
        /// </summary>
        public virtual bool RequiresParent { get { return false; } }

        /// <summary>
        /// Specifies the default layout for the document type to save having to select the layout each time a page is added. 
        /// </summary>
        public virtual string DefaultLayoutName { get { return "Default Layout"; } }

        /// <summary>
        /// Specifies the partial view used to render the View
        /// </summary>
        public virtual string EditPartialView { get { return "DefaultEdit"; } }

        /// <summary>
        /// Specifies the app that the document is a part of
        /// </summary>
        public virtual string App { get { return MrCMS.Apps.MrCMSApp.AppWebpages.ContainsKey(typeof(T)) ? MrCMS.Apps.MrCMSApp.AppWebpages[typeof(T)] : null; } }

        /// <summary>
        /// Specifies whether the children of the page are shown in the navigation tree
        /// </summary>
        public virtual bool ShowChildrenInAdminNav { get { return true; } }

        /// <summary>
        /// Specifies whether child pages will have Maintain Hierarchy checked by default
        /// </summary>
        public virtual bool ChildrenMaintainHierarchy { get { return true; } }

        /// <summary>
        /// Specifies whether child pages will have Maintain Hierarchy checked by default
        /// </summary>
        public virtual bool RevealInNavigation { get { return true; } }

        /// <summary>
        /// Specifies whether the page type is able to have body content
        /// </summary>
        public virtual bool HasBodyContent { get { return true; } }

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
                               Sortable = Sortable,
                               DisplayOrder = DisplayOrder,
                               Type = Type,
                               PostTypes = PostTypes,
                               ChildrenListType = ChildrenListType,
                               ChildrenList = ChildrenList,
                               AutoBlacklist = AutoBlacklist,
                               RequiresParent = RequiresParent,
                               DefaultLayoutName = DefaultLayoutName,
                               EditPartialView = EditPartialView,
                               ShowChildrenInAdminNav = ShowChildrenInAdminNav,
                               App = App,
                               ChildrenMaintainHierarchy = ChildrenMaintainHierarchy,
                               RevealInNavigation = RevealInNavigation,
                               HasBodyContent = HasBodyContent
                           };
            }
        }
    }
}