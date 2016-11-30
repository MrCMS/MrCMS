using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports
{
   
    public class D3UserSubscriptionReportsModel : ID3AxisComponent
    {
        public string Label { get; set; }
        public string XComponent { get; set; }
        public string YComponent { get; set; }
        public IEnumerable<object> JsonObject { get; set; }

        public D3UserSubscriptionReportsModel()
        {
            XComponent = "JoiningMonthYear";
            YComponent = "Count";
            Label = "User Subscription Month Wise";
            JsonObject = new List<JsonObject>();
        }
    }

    public class JsonObject
    {
        public string JoiningMonthYear { get; set; }
        public int Count { get; set; }
    }
}