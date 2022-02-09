namespace MrCMS.Services.ImportExport.Rules
{
    public class WebpageMaxMetaKeywordsLengthIs250 : WebpageMaxStringLength
    {
        public WebpageMaxMetaKeywordsLengthIs250()
            : base("SEO Keywords", o => o.MetaKeywords, 250)
        {
        }
    }
}