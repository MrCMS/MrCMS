using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Models
{
    public class UpdateFormModel : UpdateAdminViewModel<Form>
    {
        public string Name { get; set; }
    }
}