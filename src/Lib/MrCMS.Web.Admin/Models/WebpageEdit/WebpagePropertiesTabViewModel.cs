using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models.WebpageEdit
{
    public class WebpagePropertiesTabViewModel
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string BodyContent { get; set; }
        
        // todo - remove from core!
        public string CampaignName { get; set; }
        public string PageType { get; set; }
        public bool OnlyAtLandingPage { get; set; }
    }
}