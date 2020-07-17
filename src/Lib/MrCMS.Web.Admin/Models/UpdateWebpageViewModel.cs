using System;
using System.Collections;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Models
{
    public class UpdateWebpageViewModel : UpdateAdminViewModel<Webpage>
    {
        public DateTime? PublishOn { get; set; }
    }
}