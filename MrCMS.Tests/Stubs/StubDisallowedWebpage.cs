using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tests.Stubs
{
    public class StubDisallowedWebpage : Webpage
    {
        public override bool IsAllowed(MrCMS.Entities.People.User user)
        {
            return false;
        }
    }
}