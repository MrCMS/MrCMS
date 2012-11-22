namespace MrCMS.Entities.Documents.Web
{
    public class FormValue : BaseEntity
    {
        public virtual FormPosting FormPosting  { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}