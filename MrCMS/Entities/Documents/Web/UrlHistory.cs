using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MrCMS.Entities.Documents.Web
{
    public class UrlHistory : SiteEntity 
    {
        [Required]
        [DisplayName("Url Segment")]
        [Remote("ValidateUrlIsAllowed", "UrlHistory")]
        public virtual string UrlSegment { get; set; }

        public virtual Webpage Webpage { get; set; }
    }
}
