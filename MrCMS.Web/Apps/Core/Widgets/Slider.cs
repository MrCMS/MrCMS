using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using NHibernate;
using NHibernate.Mapping;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class Slider : Widget 
    {
        [DisplayName("Slider 1")]
        public virtual string Image { get; set; } //reusing image and link as per user links
        [DisplayName("Link for Slider 1")]
        public virtual string Link { get; set; }
        [DisplayName("Description for Slider 1")]
        [AllowHtml]
        public virtual string Description { get; set; }

        [DisplayName("Slider 2")]
        public virtual string Image1 { get; set; }
        [DisplayName("Link for Slider 2")]
        public virtual string Link1 { get; set; }
        [DisplayName("Description for Slider 2")]
        [AllowHtml]
        public virtual string Description1 { get; set; }

        [DisplayName("Slider 3")]
        public virtual string Image2 { get; set; }
        [DisplayName("Link for Slider 3")]
        public virtual string Link2 { get; set; }
        [DisplayName("Description for Slider 3")]
        [AllowHtml]
        public virtual string Description2 { get; set; }

        [DisplayName("Slider 4")]
        public virtual string Image3 { get; set; }
        [DisplayName("Link for Slider 4")]
        public virtual string Link3 { get; set; }
        [DisplayName("Description for Slider 4")]
        [AllowHtml]
        public virtual string Description3 { get; set; }

        public virtual IEnumerable<SliderItem> Sliders
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Image))
                    yield return new SliderItem { Description = Description, Image = Image, Link = Link };
                if (!string.IsNullOrWhiteSpace(Image1))
                    yield return new SliderItem { Description = Description1, Image = Image1, Link = Link1 };
                if (!string.IsNullOrWhiteSpace(Image2))
                    yield return new SliderItem { Description = Description2, Image = Image2, Link = Link2 };
                if (!string.IsNullOrWhiteSpace(Image3))
                    yield return new SliderItem { Description = Description3, Image = Image3, Link = Link3 };
            }
        }

    }
    public class SliderItem
    {
        public string Image { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
    }

}