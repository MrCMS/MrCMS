namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class DropDownList : FormPropertyWithOptions
    {
        public override bool OnlyOneOptionSelectable
        {
            get { return true; }
        }
    }
}