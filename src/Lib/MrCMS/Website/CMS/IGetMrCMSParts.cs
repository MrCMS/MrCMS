using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using MrCMS.Apps;

namespace MrCMS.Website.CMS
{
    public interface IGetMrCMSParts
    {
        IEnumerable<RegistrationInfo> GetSortedMiddleware(MrCMSAppContext appContext,
            Action<IApplicationBuilder> coreFunctions);
    }
}