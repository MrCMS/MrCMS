using System.Web.Mvc;

namespace MrCMS.Services.Canonical
{
    public static class ViewDataExtensions
    {
        public const string LinkTagsKey = "current.request.linktags";

        public static LinkTagDictionary LinkTags(this ViewDataDictionary viewData)
        {
            if (!viewData.ContainsKey(LinkTagsKey))
            {
                viewData[LinkTagsKey] = new LinkTagDictionary();
            }
            return viewData[LinkTagsKey] as LinkTagDictionary;
        }
    }
}