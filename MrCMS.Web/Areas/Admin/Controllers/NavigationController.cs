using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NavigationController : AdminController
    {
        private readonly INavigationService _service;
        private readonly IUserService _userService;

        public NavigationController(INavigationService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        public PartialViewResult WebSiteTree()
        {
            return PartialView("WebSiteTree", _service.GetWebsiteTree());
        }

        public PartialViewResult MediaTree()
        {
            return PartialView("MediaTree", _service.GetMediaTree());
        }

        public PartialViewResult LayoutTree()
        {
            return PartialView("LayoutTree", _service.GetLayoutList());
        }

        public PartialViewResult UserList()
        {
            return PartialView("UserList", _service.GetUserList());
        }

        public PartialViewResult GetMore(int parentId, int previous)
        {
            var more = _service.GetMore(parentId, previous);

            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult LoggedInAs()
        {
            User user = _userService.GetCurrentUser(HttpContext);
            return PartialView(user);
        }

        //public MvcHtmlString GetUserList()
        //{
        //    var rootUl = new TagBuilder("ul");
        //    rootUl.Attributes.Add("class", "user-list");
        //    var rootLi = new TagBuilder("li");
        //    int rootId = 1;
        //    rootLi.Attributes.Add("data-id", rootId.ToString());
        //    rootLi.Attributes.Add("data-controller", "User");
        //    rootLi.InnerHtml += string.Format("<i class=\"{0}\">&nbsp;</i>Root", DocumentTypeHelper.GetRootIconClass());
        //    var userList = new TagBuilder("ul");
        //    foreach (var user in UserService.GetAllUsers())
        //    {
        //        var listItem = new TagBuilder("li");
        //        listItem.Attributes.Add("data-id", user.Id.ToString(CultureInfo.InvariantCulture));
        //        listItem.Attributes.Add("data-controller", typeof(User).Name);
        //        listItem.InnerHtml += string.Format("<i class=\"icon-user\">&nbsp;</i>");
        //        listItem.InnerHtml += !string.IsNullOrWhiteSpace(user.Name) ? user.Name : user.Email;

        //        userList.InnerHtml += listItem.ToString();
        //    }

        //    rootLi.InnerHtml += userList.ToString();

        //    rootUl.InnerHtml = rootLi.ToString();
        //    return MvcHtmlString.Create(rootUl.ToString()); //PartialView("UserList", _userService.GetUsers(1,1));
        //}

        //protected MvcHtmlString GetCategories<T>() where T : Document
        //{
        //    var rootUl = new TagBuilder("ul");

        //    rootUl.Attributes.Add("class", "filetree treeview-famfamfam browser");

        //    var rootLi = new TagBuilder("li");
        //    int rootId = 1;
        //    rootLi.Attributes.Add("data-id", rootId.ToString());
        //    rootLi.Attributes.Add("data-controller", typeof(T).Name);
        //    rootLi.InnerHtml += string.Format("<i class=\"{0}\">&nbsp;</i>", DocumentTypeHelper.GetRootIconClass());

        //    var link = new TagBuilder("a");
        //    link.Attributes.Add("href", Url.Action("View", typeof(T).Name, new { id = rootId }));
        //    link.InnerHtml = "Root";
        //    rootLi.InnerHtml += link.ToString();

        //    var documents = DocumentService.GetDocumentsByParentId<T>(rootId);
        //    rootLi.InnerHtml += GetNavList<T>(documents);
        //    rootUl.InnerHtml = rootLi.ToString();

        //    return MvcHtmlString.Create(rootUl.ToString());
        //}

        //private string GetNavList<T>(IEnumerable<Document> documents) where T : Document
        //{
        //    var tagBuilder = new TagBuilder("ul");
        //    foreach (var document in documents)
        //    {
        //        var listItem = new TagBuilder("li");
        //        listItem.Attributes.Add("data-id", document.Id.ToString(CultureInfo.InvariantCulture));
        //        listItem.Attributes.Add("data-controller", typeof(T).Name);
        //        listItem.InnerHtml += string.Format("<i class=\"{0}\">&nbsp;</i>", DocumentTypeHelper.GetIconClass(document));

        //        var link = new TagBuilder("a");
        //        link.Attributes.Add("href", Url.Action("View", typeof (T).Name, new {id = document.Id}));
        //        link.InnerHtml = document.Name;
        //        listItem.InnerHtml += link.ToString();

        //        var children = DocumentService.GetDocumentsByParentId<T>(document.Id);
        //        if (children != null && children.Any())
        //        {
        //            listItem.InnerHtml += GetNavList<T>(children);
        //        }
        //        tagBuilder.InnerHtml += listItem.ToString();
        //    }
        //    return tagBuilder.ToString();
        //}
    }
}