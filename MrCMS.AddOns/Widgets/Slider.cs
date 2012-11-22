using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class Slider : Widget
    {
        public virtual IList<SliderItem> Slides { get; set; }
    }

    [MrCMSMapClass]
    public class SliderItem : MrCMS.Entities.BaseEntity
    {
        public virtual string Title { get; set; }
        public virtual string Image { get; set; }
        public virtual string Url { get; set; }
        public virtual int Order { get; set; }

        public virtual Slider Slider { get; set; }
    }

    public class SliderOverride : IAutoMappingOverride<Slider>
    {
        public void Override(AutoMapping<Slider> mapping)
        {
            mapping.HasMany(slider => slider.Slides).Cascade.All();
        }
    }

    public class SliderService
    {
        private readonly ISession _session;

        public SliderService(ISession session)
        {
            _session = session;
        }

        public SliderItem SaveSlider(SliderItem item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
            return item;
        }

        public void DeleteSlider(SliderItem item)
        {
            _session.Transact(session => session.Delete(item));
        }

        public IList<SliderItem> GetSliders()
        {
            return _session.QueryOver<SliderItem>().List();
        }
    }
}