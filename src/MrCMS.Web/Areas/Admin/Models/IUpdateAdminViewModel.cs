using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models
{
    public interface IUpdateAdminViewModel
    {
        int Id { get; set; }
        ICollection<object> Models { get; set; }
    }
}