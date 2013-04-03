using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;

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