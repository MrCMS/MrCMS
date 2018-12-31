using Microsoft.AspNetCore.Http;

namespace MrCMS.Website.CMS
{
    public class CmsMethodTester : ICmsMethodTester
    {
        public bool IsRoutable(string httpMethod)
        {
            return HttpMethods.IsGet(httpMethod) || HttpMethods.IsHead(httpMethod) || HttpMethods.IsPost(httpMethod);
        }
    }
}