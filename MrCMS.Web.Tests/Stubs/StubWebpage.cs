using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Tests.Stubs
{
    public class StubWebpage : Webpage
    {
        public virtual void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions;
        }
    }
}