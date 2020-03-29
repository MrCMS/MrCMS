using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class UpdateWebpageViewModel : UpdateAdminViewModel<Webpage>
    {
        public DateTime? PublishOn { get; set; }
    }
}