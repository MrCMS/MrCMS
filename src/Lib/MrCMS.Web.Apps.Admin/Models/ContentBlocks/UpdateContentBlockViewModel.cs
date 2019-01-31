using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models.ContentBlocks
{
    public class UpdateContentBlockViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int WebpageId { get; set; }
    }
}