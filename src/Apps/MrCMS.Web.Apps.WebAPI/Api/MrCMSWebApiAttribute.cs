using System;
using System.Collections.Generic;
using System.Text;

namespace MrCMS.Web.Apps.WebApi.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MrCMSWebApiAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MrCMSAdminWebApiAttribute : Attribute
    {

    }


}
