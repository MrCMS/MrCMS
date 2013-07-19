namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentMaxNameLengthIs255 : DocumentMaxStringLength
    {
        public DocumentMaxNameLengthIs255()
            : base("Name", o => o.Name, 255)
        {
        }
    }
}