using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Tests.Stubs
{
    public class StubDocument : Document
    {
        public void SetChildren(IList<Document> children)
        {
            Children = children;
            foreach (var document in Children)
            {
                document.Parent = this;
            }
        }

        public void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions;
        }
    }
}