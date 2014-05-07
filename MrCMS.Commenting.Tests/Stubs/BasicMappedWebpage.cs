using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Commenting.Tests.Stubs
{
    [MrCMSMapClass]
    public class BasicMappedWebpage : Webpage
    {
        public BasicMappedWebpage()
        {
            Widgets = new List<Widget>();
        }
    }
    [MrCMSMapClass]
    public class BasicMappedWebpage2 : Webpage
    {
        public BasicMappedWebpage2()
        {
            Widgets = new List<Widget>();
        }
    }
}