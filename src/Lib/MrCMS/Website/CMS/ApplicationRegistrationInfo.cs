using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.CMS
{
    public class ApplicationRegistrationInfo
    {
        public int Order { get; set; }
        public Action<IApplicationBuilder> Registration { get; set; }
    }

    public class EndpointRegistrationInfo
    {
        public int Order { get; set; }
        public Action<IEndpointRouteBuilder> Registration { get; set; }
    }
}