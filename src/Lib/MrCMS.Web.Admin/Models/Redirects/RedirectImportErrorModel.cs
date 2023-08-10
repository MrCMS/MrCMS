namespace MrCMS.Web.Admin.Models.Redirects;

public class RedirectImportErrorModel
{
    public RedirectImportErrorModel(RedirectImportModel data)
    {
        OldUrl = data.OldUrl;
        NewUrl = data.NewUrl;
    }

    public RedirectImportErrorModel()
    {
    }
    
    public string OldUrl { get; set; }

    public string NewUrl { get; set; }

    public RedirectImportErrorType Error { get; set; }
}

public enum RedirectImportErrorType
{
    EmptyUrl,
    AbsoluteUrl,
    DuplicatedUrl,
    NotUniqueUrl
}