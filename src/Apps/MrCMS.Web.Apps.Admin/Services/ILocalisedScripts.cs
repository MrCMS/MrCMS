using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ILocalisedScripts
    {
        IEnumerable<string> Files { get; }
    }
}