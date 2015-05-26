using System;
using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports
{
    interface ID3AxisComponent
    {
        string Label { get; set; }
        string XComponent { get; set; }
        string YComponent { get; set; }
        IEnumerable<object> JsonObject { get; set; }
    }
}
