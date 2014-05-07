using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Commenting.Areas.Admin.Controllers
{
    public class CommentController : MrCMSAppAdminController<CommentingApp>
    {
        private readonly ICommentAdminService _commentAdminService;

        public CommentController(ICommentAdminService commentAdminService)
        {
            _commentAdminService = commentAdminService;
        }

        public ViewResult Index([IoCModelBinder(typeof(CommentSearchQueryModelBinder))]CommentSearchQuery searchQuery)
        {
            ViewData["approval-options"] = _commentAdminService.GetApprovalOptions();
            ViewData["results"] = _commentAdminService.Search(searchQuery);
            return View(searchQuery);
        }

        public ViewResult Show(Comment comment)
        {
            return View(comment);
        }

        [HttpPost]
        public RedirectToRouteResult Approval([IoCModelBinder(typeof(CommentApprovalModelBinder))]Comment comment)
        {
            _commentAdminService.Update(comment);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Delete(Comment comment)
        {
            return View(comment);
        }
        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(Comment comment)
        {
            _commentAdminService.Delete(comment);
            return RedirectToAction("Index");
        }
    }
    public class CommentSearchQueryModelBinder : MrCMSDefaultModelBinder
    {
        private readonly HttpSessionStateBase _sessionStateBase;
        private const string CommentQueryKey = "admin.commentquery";

        public CommentSearchQueryModelBinder(IKernel kernel)
            : base(kernel)
        {
            _sessionStateBase = kernel.Get<HttpSessionStateBase>();
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is CommentSearchQuery)
                _sessionStateBase[CommentQueryKey] = bindModel;
            return bindModel;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType != typeof(CommentSearchQuery))
                return base.CreateModel(controllerContext, bindingContext, modelType);
            var commentSearchQuery = !controllerContext.HttpContext.Request.QueryString.HasKeys()
                                 ? _sessionStateBase[CommentQueryKey] as CommentSearchQuery ?? new CommentSearchQuery()
                                 : new CommentSearchQuery();
            commentSearchQuery.Page = 1;
            return commentSearchQuery;
        }
    }
}