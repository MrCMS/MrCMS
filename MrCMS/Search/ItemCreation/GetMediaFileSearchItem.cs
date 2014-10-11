using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetMediaFileSearchItem : GetUniversalSearchItemBase<MediaFile>
    {
        private readonly UrlHelper _urlHelper;

        public GetMediaFileSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(MediaFile mediaFile)
        {
            return new UniversalSearchItem
                       {
                           DisplayName = mediaFile.FileName,
                           Id = mediaFile.Id,
                           SearchTerms = new string[] {mediaFile.FileName , mediaFile.FileExtension, mediaFile.FileUrl},
                           SystemType = mediaFile.GetType().FullName,
                           ActionUrl = _urlHelper.Action("Edit", "MediaFile", new { id = mediaFile.Id }),
                       };
        }
    }
}