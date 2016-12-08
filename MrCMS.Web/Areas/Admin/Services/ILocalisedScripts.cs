using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILocalisedScripts
    {
        IEnumerable<string> Files { get; }
    }
}