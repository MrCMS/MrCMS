namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentMaxMetaTitleLengthIs250 : DocumentMaxStringLength
    {
        public DocumentMaxMetaTitleLengthIs250()
            : base("SEO Title", o => o.MetaTitle, 250)
        {
        }
    }
}