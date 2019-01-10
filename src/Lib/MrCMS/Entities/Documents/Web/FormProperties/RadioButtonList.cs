namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class RadioButtonList : FormPropertyWithOptions
    {
        public override bool OnlyOneOptionSelectable
        {
            get { return true; }
        }
    }
}