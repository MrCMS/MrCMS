using System;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class SaveResult
    {
        public SaveResult()
        {
            success = true;
            message = String.Empty;
        }

        public SaveResult(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }

        public bool success { get; set; }
        public string message { get; set; }
    }
}