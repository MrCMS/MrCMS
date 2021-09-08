using System.Collections.Generic;

namespace MrCMS.Web.Admin.Infrastructure.Models
{
    public interface IUpdateAdminViewModel
    {
        int Id { get; set; }
        ICollection<object> Models { get; set; }
    }
}