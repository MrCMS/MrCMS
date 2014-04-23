using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface ICommentAdminService
    {
        IPagedList<Comment> Search(CommentSearchQuery query);
        List<SelectListItem> GetApprovalOptions();
        void Update(Comment comment);
        void Delete(Comment comment);
    }
}