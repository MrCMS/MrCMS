using System;
using System.Collections;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class UpdateWebpageViewModel : UpdateAdminViewModel<Webpage>
    {
        public DateTime? PublishOn { get; set; }
    }
}