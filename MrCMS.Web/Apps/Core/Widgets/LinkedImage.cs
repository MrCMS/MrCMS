using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Widgets
{
    [MrCMSMapClass]
    public class LinkedImage : Widget 
    {
        public virtual string Image { get; set; }
        public virtual string Link { get; set; }
    }
}