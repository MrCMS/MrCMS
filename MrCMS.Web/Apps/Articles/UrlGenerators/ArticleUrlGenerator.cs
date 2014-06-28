using System.Text;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Articles.UrlGenerators
{
    public class ArticleUrlGenerator : WebpageUrlGenerator<Article>
    {
        public override string GetUrl(string pageName, Webpage parent, bool useHierarchy)
        {
            var stringBuilder = new StringBuilder();

            if (useHierarchy)
            {
                //get breadcrumb from parent
                if (parent != null)
                {
                    stringBuilder.Insert(0, SeoHelper.TidyUrl(parent.UrlSegment) + "/");
                }
            }
            //add page name

            stringBuilder.AppendFormat("{0:yyyy/MM/}", CurrentRequestData.Now);
            stringBuilder.Append(SeoHelper.TidyUrl(pageName));
            return stringBuilder.ToString();
        }
    }
}