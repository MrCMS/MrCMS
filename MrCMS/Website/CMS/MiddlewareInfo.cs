using System;
using Microsoft.AspNetCore.Builder;

namespace MrCMS.Website.CMS
{
    public class MiddlewareInfo
    {
        public int Order { get; set; }
        public Action<IApplicationBuilder> Registration { get; set; }
    }
}