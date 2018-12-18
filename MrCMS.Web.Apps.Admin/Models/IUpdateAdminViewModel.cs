using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Models
{
    public interface IUpdateAdminViewModel
    {
        int Id { get; set; }
        ICollection<object> Models { get; set; }
    }
}