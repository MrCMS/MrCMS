using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public class MrCMSStandardRouteHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly IHandleStandardMrCMSPageExecution _standardMrCMSPageExecution;

        public MrCMSStandardRouteHandler(IGetWebpageForRequest getWebpageForRequest,
            IHandleStandardMrCMSPageExecution standardMrCMSPageExecution)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _standardMrCMSPageExecution = standardMrCMSPageExecution;
        }

        public int Priority
        {
            get { return 1000; }
        }

        public bool Handle(RequestContext context)
        {
            Webpage webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
            {
                return false;
            }
            _standardMrCMSPageExecution.Handle(context, webpage);
            return true;
        }
    }
}