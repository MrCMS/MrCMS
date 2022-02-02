namespace MrCMS.Services.ImportExport.Rules
{
    public class WebpageMaxMetaTitleLengthIs250 : WebpageMaxStringLength
    {
        public WebpageMaxMetaTitleLengthIs250()
            : base("SEO Title", o => o.MetaTitle, 250)
        {
        }
    }
}