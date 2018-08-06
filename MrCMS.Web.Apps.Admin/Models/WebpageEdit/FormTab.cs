using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class FormTab : AdminTabGroup<Webpage>
    {
        public override int Order
        {
            get { return 300; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Form";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return true;
        }
    }
}