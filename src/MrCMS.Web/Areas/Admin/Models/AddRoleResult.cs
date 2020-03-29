namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddRoleResult
    {
        public AddRoleResult(bool success, string error)
        {
            Error = error;
            Success = success;
        }

        public bool Success { get; private set; }
        public string Error { get; private set; }
    }
}