using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tests.Stubs
{
    [MrCMSMapClass]
    public class BasicMappedWebpage : Webpage
    {
    }

    public class BasicMappedNoChildrenInNavWebpage : Webpage
    {
        
    }

    public class BasicMappedNoChildrenInNavWebpageMetadataMap : DocumentMetadataMap<BasicMappedNoChildrenInNavWebpage>
    {
        public override bool ShowChildrenInAdminNav
        {
            get { return false; }
        }   
    }
}