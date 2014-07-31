using MrCMS.Entities.Documents;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class DocumentTypeToAdd
    {
        public DocumentMetadata Type { get; set; }
        public int? TemplateId { get; set; }
        public string DisplayName { get; set; }

        public string DisplayId
        {
            get { return TemplateId.HasValue ? Type.Type.FullName + "-" + TemplateId : Type.Type.FullName; }
        }
    }
}