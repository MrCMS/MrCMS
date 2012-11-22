using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Models
{
    public class RoleModel
    {
        public string Name { get; set; }

        public RoleStatus Status { get; set; }
        public IEnumerable<SelectListItem> GetStatusOptions
        {
            get
            {
                return
                    Enum.GetValues(typeof(RoleStatus)).Cast<RoleStatus>().BuildSelectItemList(
                        status => status.ToString(),
                        status => status.ToString(),
                        status => status == Status, (string)null);
            }
        }
        public bool? Recursive { get; set; }
        public IEnumerable<SelectListItem> GetRecursiveOptions
        {
            get
            {
                return
                    new List<bool?> { true, false, null }.BuildSelectItemList(
                        option => option == null ? "Inherit" : option.ToString(),
                        option => option.ToString(),
                        option => option == Recursive, (string)null);
            }
        }
    }
}