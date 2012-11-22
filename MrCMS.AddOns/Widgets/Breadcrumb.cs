using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class Breadcrumb : Widget
    {
        public override bool HasProperties
        {
            get { return false; }
        }
    }
}
