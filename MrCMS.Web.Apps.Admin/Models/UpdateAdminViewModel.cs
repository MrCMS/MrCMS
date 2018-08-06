using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Models
{
    public interface IUpdateAdminViewModel
    {
        int Id { get; set; }
        ICollection<object> Models { get; set; }
    }

    public abstract class UpdateAdminViewModel<T> : IUpdateAdminViewModel where T : SystemEntity
    {
        public int Id { get; set; }
        public ICollection<object> Models { get; set; }
    }
}