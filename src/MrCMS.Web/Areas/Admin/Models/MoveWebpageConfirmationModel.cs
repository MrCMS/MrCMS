using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MoveWebpageConfirmationModel
    {
        public Webpage Webpage { get; set; }
        public Webpage Parent { get; set; }

        public List<MoveWebpageChangedPageModel> ChangedPages { get; set; }
    }
}