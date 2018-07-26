namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentMaxMetaKeywordsLengthIs250 : DocumentMaxStringLength
    {
        public DocumentMaxMetaKeywordsLengthIs250()
            : base("SEO Keywords", o => o.MetaKeywords, 250)
        {
        }
    }
}