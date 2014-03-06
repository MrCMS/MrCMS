using System.ComponentModel;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Models
{
    public class MediaCategorySearchModel
    {
        public MediaCategorySearchModel()
        {
            Page = 1;
        }

        public int Id { get; set; }
        public int Page { get; set; }
        [DisplayName("Search files")]
        public string SearchText { get; set; }

    }
}