using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tests.Stubs
{
    public class StubAllowedWebpage : Webpage
    {
        public override bool IsAllowed(MrCMS.Entities.People.User user)
        {
            return true;
        }
    }
}