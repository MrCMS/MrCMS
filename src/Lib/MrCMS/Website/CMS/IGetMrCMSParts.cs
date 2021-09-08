using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MrCMS.Apps;

namespace MrCMS.Website.CMS
{
    public interface IGetMrCMSParts
    {
        IEnumerable<ApplicationRegistrationInfo> GetSortedMiddleware(MrCMSAppContext appContext,
            Action<IApplicationBuilder> coreFunctions);

        IEnumerable<EndpointRegistrationInfo> GetSortedEndpoints(MrCMSAppContext appContext,
            Action<IEndpointRouteBuilder> coreFunctions);
    }
}