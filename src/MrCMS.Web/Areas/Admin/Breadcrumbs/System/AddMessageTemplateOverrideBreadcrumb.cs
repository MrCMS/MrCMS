﻿using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class AddMessageTemplateOverrideBreadcrumb : Breadcrumb<MessageTemplateBreadcrumb>
    {
        public override string Controller => "MessageTemplate";
        public override string Action => "AddSiteOverride";
        public override string Name => "Add Site Override";
        public override Task Populate()
        {
            ParentActionArguments = ActionArguments;
            return Task.CompletedTask;
        }
    }
}