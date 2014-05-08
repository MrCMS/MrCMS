using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using System.Linq;

namespace MrCMS.Tests.Stubs
{
    public class StubWebpage : Webpage
    {
        public StubWebpage()
        {
            FormProperties = new List<FormProperty>();
        }
    }
}