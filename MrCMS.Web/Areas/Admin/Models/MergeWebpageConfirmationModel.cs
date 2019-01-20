using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MergeWebpageConfirmationModel
    {
        public Webpage Webpage { get; set; }
        public Webpage MergedInto { get; set; }

        public List<MergeWebpageChangedPageModel> ChangedPages { get; set; }
    }
}