using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Tests.Stubs
{
    public class StubDocument : Document
    {
        public virtual void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions;
        }
    }
}