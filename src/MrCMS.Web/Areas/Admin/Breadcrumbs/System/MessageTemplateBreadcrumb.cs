﻿using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class MessageTemplateBreadcrumb : Breadcrumb<MessageTemplatesBreadcrumb>
    {
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;

        public override Task Populate()
        {
            var type = ActionArguments["type"] as string;
            var typeByName = TypeHelper.GetTypeByName(type);
            Name = typeByName.Name.BreakUpString();
            return Task.CompletedTask;
        }
    }
}