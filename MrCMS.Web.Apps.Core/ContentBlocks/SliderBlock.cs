using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Web.Apps.Core.ContentBlocks
{
    public class SliderBlock : ContentBlock
    {
        public virtual string Image { get; set; }
        public virtual string Image2 { get; set; }
        public virtual string Image3 { get; set; }
        public virtual string Image4 { get; set; }
        public virtual string Image5 { get; set; }
        public virtual string Image6 { get; set; }

        public virtual string Link { get; set; }
        public virtual string Link2 { get; set; }
        public virtual string Link3 { get; set; }
        public virtual string Link4 { get; set; }
        public virtual string Link5 { get; set; }
        public virtual string Link6 { get; set; }

        public virtual IEnumerable<SliderItem> Sliders
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Image))
                    yield return new SliderItem { Image = Image, Link = Link};
                if (!string.IsNullOrWhiteSpace(Image2))
                    yield return new SliderItem { Image = Image2, Link = Link2 };
                if (!string.IsNullOrWhiteSpace(Image3))
                    yield return new SliderItem { Image = Image3, Link = Link3 };
                if (!string.IsNullOrWhiteSpace(Image4))
                    yield return new SliderItem { Image = Image4, Link = Link4 };
                if (!string.IsNullOrWhiteSpace(Image5))
                    yield return new SliderItem { Image = Image5, Link = Link5 };
                if (!string.IsNullOrWhiteSpace(Image6))
                    yield return new SliderItem { Image = Image6, Link = Link6 };
            }
        }
        public class SliderItem
        {
            public string Image { get; set; }
            public string Link { get; set; }
            public string Description { get; set; }
        }
    }
}