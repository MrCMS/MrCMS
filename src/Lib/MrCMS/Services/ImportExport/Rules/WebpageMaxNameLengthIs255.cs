namespace MrCMS.Services.ImportExport.Rules
{
    public class WebpageMaxNameLengthIs255 : WebpageMaxStringLength
    {
        public WebpageMaxNameLengthIs255()
            : base("Name", o => o.Name, 255)
        {
        }
    }
}