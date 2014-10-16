using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class FormTab : WebpageTabGroup
    {
        public override int Order
        {
            get { return 300; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string Name(Webpage webpage)
        {
            return "Form";
        }

        public override bool ShouldShow(Webpage webpage)
        {
            return true;
        }
    }
}