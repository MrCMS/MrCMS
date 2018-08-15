using System.Collections.Generic;
using MrCMS.Apps;

namespace MrCMS.Website.CMS
{
    public interface IGetMrCMSMiddleware
    {
        IEnumerable<MiddlewareInfo> GetSortedMiddleware(MrCMSAppContext appContext);
    }
}