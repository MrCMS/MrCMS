namespace MrCMS.Services.ImportExport.Rules
{
    public class WebpageMaxMetaDescriptionLengthIs250 : WebpageMaxStringLength
    {
        public WebpageMaxMetaDescriptionLengthIs250()
            : base("SEO Description", o => o.MetaDescription, 250)
        {
        }
    }
}