using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content
{
    public class UpdateEmbedVideoAdminModel
    {
        [DisplayName("Embed Code"), Required]
        public string EmbedCode { get; set; }

        public string Poster { get; set; }
    }
}