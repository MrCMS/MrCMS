using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tests.Stubs
{
    [MrCMSMapClass]
    public class StubDocument : Document
    {
        public virtual void SetChildren(IList<Document> children)
        {
            Children = children;
            foreach (var document in Children)
            {
                document.Parent = this;
            }
        }

        public virtual void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions;
        }
    }
    [MrCMSMapClass]
    public class StubUniquePage : IUniquePage
    {
    }
}