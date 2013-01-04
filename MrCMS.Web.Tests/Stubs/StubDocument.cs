using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Tests.Stubs
{
    public class StubDocument : Document
    {
        public void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions;
        }
    }
}