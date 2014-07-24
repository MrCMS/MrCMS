using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iesi.Collections.Generic;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class FeatureSection : TextPage
    {
        public FeatureSection()
        {
            FeaturesInOtherSections = new HashedSet<Feature>();
        }

        [DisplayName("Page Size")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Page size must be a number")]
        public virtual int PageSize { get; set; }

        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }

        public virtual ISet<Feature> FeaturesInOtherSections { get; set; }
    }
}