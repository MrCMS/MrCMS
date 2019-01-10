namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentMaxMetaDescriptionLengthIs250 : DocumentMaxStringLength
    {
        public DocumentMaxMetaDescriptionLengthIs250()
            : base("SEO Description", o => o.MetaDescription, 250)
        {
        }
    }
}