namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class CheckboxList : FormPropertyWithOptions
    {
        public override bool OnlyOneOptionSelectable
        {
            get { return false; }
        }
    }
}