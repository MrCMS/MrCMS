using System.Collections.Generic;

namespace MrCMS.Web.Admin.Services
{
    public interface ILocalisedScripts
    {
        IEnumerable<string> Files { get; }
    }
}