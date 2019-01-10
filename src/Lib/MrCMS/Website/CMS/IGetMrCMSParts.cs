using System.Collections.Generic;
using MrCMS.Apps;

namespace MrCMS.Website.CMS
{
    public interface IGetMrCMSParts
    {
        IEnumerable<RegistrationInfo> GetSortedMiddleware(MrCMSAppContext appContext);
    }
}