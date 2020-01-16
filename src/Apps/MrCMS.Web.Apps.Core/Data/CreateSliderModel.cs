using Microsoft.EntityFrameworkCore;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Data
{
    public class CreateSliderModel  :ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Slider>(slider =>
            {
                slider.HasBaseType<Widget>();
                slider.Ignore(x => x.Sliders);
            });
        }
    }
}