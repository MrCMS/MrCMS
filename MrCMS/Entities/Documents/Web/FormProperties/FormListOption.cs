namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class FormListOption : SiteEntity
    {
        public virtual FormProperty FormProperty { get; set; }
        public virtual string Value { get; set; }
        public virtual bool Selected { get; set; }
    }
}