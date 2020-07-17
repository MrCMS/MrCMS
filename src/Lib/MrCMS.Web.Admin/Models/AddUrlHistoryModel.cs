using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Admin.Models
{
    public class AddUrlHistoryModel
    {
        public int WebpageId { get; set; }

        [Required]
        [DisplayName("Url Segment")]
        [Remote("ValidateUrlIsAllowed", "UrlHistory")]
        public string UrlSegment { get; set; }
    }
}