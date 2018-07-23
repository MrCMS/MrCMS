using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData : IGetPageData
    {
        //private readonly IRoutingDataRepository _repository;
        //private readonly IGetWebpageRegistration _getWebpageRegistration;
        //private readonly ICanPreviewWebpage _canPreview;
        //private readonly ICMSDescriptorChecker _descriptorChecker;

        //public GetPageData(IRoutingDataRepository repository, IGetWebpageRegistration getWebpageRegistration, ICanPreviewWebpage canPreview, ICMSDescriptorChecker descriptorChecker)
        //{
        //    _repository = repository;
        //    _getWebpageRegistration = getWebpageRegistration;
        //    _canPreview = canPreview;
        //    _descriptorChecker = descriptorChecker;
        //}
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;
        private readonly IGetHomePage _getHomePage;

        public GetPageData(IGetDocumentByUrl<Webpage> getWebpageByUrl, IGetHomePage getHomePage)
        {
            _getWebpageByUrl = getWebpageByUrl;
            _getHomePage = getHomePage;
        }


        public async Task<PageData> GetData(string url)
        {
            Webpage webpage = string.IsNullOrWhiteSpace(url)
                ? _getHomePage.Get()
                : _getWebpageByUrl.GetByUrl(url);
            //var webpage = await _repository.GetByUrl(url);
            if (webpage == null)
                return null;

            // TODO: additional checks for page data
            //var isPreview = !webpage.Published;
            //if (isPreview && !await _canPreview.CanPreview(webpage))
            //    return null;

            //var mvcData = _getWebpageRegistration.GetRegistration(webpage);
            //if (mvcData == null)
            //    return null;

            //var actionExists = _descriptorChecker.HasMatchingDescriptor(mvcData.Action, mvcData.Controller);

            var metadata = webpage.GetMetadata();

            return new PageData
            {
                IsPreview = false,
                Id = webpage.Id,
                Type = webpage.GetType(),
                Controller = metadata.WebGetController,
                Action = metadata.WebGetAction
                //Controller = actionExists ? mvcData.Controller : "Webpage",
                //Action = actionExists ? mvcData.Action : "Show"
            };
        }
    }
}