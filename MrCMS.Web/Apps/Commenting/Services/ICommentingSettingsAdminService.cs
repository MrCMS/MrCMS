using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.Settings;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface ICommentingSettingsAdminService
    {
        List<Type> GetAllPageTypes();
        void UpdateSettings(CommentingSettings settings);
        CommentingSettings GetSettings();
        List<SelectListItem> GetCommentApprovalTypes();
    }
}