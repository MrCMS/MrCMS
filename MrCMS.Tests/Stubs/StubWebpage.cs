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
        public virtual void SetChildren(IList<Webpage> children)
        {
            Children = children.OfType<Document>().ToList();
            foreach (var document in Children)
            {
                document.Parent = this;
            }
        }
    }
}