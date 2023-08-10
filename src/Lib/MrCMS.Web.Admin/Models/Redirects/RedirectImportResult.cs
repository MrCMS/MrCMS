using System.Collections.Generic;

namespace MrCMS.Web.Admin.Models.Redirects;

public class RedirectImportResult
{
    public RedirectImportResult()
    {
        Errors = new List<RedirectImportErrorModel>();
        ImportedCount = 0;
    }
    
    public int ImportedCount { get; set; }
    public List<RedirectImportErrorModel> Errors { get; set; }
}