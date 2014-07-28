using System.ComponentModel;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class DefaultGeneratorInfo
    {
        public string PageTypeName { get; set; }
        [DisplayName("Generator")]
        public string GeneratorTypeName { get; set; }
    }
}